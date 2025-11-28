using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;
using PatientService.Domain.Interfaces.TestOrderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.TestOrderUC.TestOrderResult.Command.CreateTestOrderResult
{
    public class CreateTestOrderResultHandler : IRequestHandler<CreateTestOrderResultCommand>
    {
        private readonly ITestResultRepository _testResultRepository;
        private readonly IFlaggingSetRepository _flaggingSetRepository;
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly ITestResultDetailRepository _testResultDetailRepository;
        private readonly Random _random = new Random();
        private readonly IPatientRepository _patientRepository;

        // Danh sách đầy đủ 8 loại xét nghiệm - MỖI TEST RESULT ĐỀU CÓ TẤT CẢ 8 TIÊU CHÍ
        private readonly List<string> _testTypes = new List<string>
        {
            "WBC",  // White Blood Cell Count
            "RBC",  // Red Blood Cell Count
            "HGB",  // Hemoglobin
            "HCT",  // Hematocrit
            "PLT",  // Platelet Count
            "MCV",  // Mean Corpuscular Volume
            "MCH",  // Mean Corpuscular Hemoglobin
            "MCHC"  // Mean Corpuscular Hemoglobin Concentration
        };

        public CreateTestOrderResultHandler(
            ITestResultRepository testResultRepository,
            IFlaggingSetRepository flaggingSetRepository,
            ITestOrderRepository testOrderRepository,
            ITestResultDetailRepository testResultDetailRepository,
            IPatientRepository patientRepository)
        {
            _flaggingSetRepository = flaggingSetRepository;
            _testOrderRepository = testOrderRepository;
            _testResultRepository = testResultRepository;
            _testResultDetailRepository = testResultDetailRepository;
            _patientRepository = patientRepository;
        }

        public async Task Handle(CreateTestOrderResultCommand request, CancellationToken cancellationToken)
        {
            var testOrder = await _testOrderRepository.GetByIdWithResultsAsync(request.TestOrderId, cancellationToken)
                ?? throw new ArgumentException("Test order not found");
            var patient = await _patientRepository.GetPatientByIdAsync(testOrder.PatientId)
              ?? throw new ArgumentException("Patient not found");
            // Lấy tất cả flagging configs từ database
            var allFlaggingConfigs = await _flaggingSetRepository.GetAllAsync(cancellationToken);
            var flaggingConfigDict = allFlaggingConfigs.ToDictionary(f => f.TestName, f => f);

            // Tạo TestResult chính
            var testResult = new TestResult
            {
                ResultId = Guid.NewGuid(),
                TestOrderId = request.TestOrderId,
                TestName = "Blood Test",
                CreatedAt = DateTime.UtcNow,
                InstrumentUsed = "AutoGen Instrument"
            };
            patient.LastTestDate = DateTime.UtcNow;
            await _patientRepository.UpdatePatient(patient);
            await _testResultRepository.AddTestResultAsync(testResult);
            await _testResultRepository.SaveChangesAsync();

            // ⭐ TẠO TẤT CẢ 8 TESTRESULTDETAILS CHO MỖI TEST RESULT
            foreach (var testType in _testTypes)
            {
                // Tìm flagging config phù hợp dựa trên test type và giới tính
                FlaggingSetConfig? config = null;
                string fullTestName = GetFullTestName(testType, testOrder.Gender);
                
                // Thử tìm config theo giới tính trước (cho RBC, HGB, HCT)
                if (flaggingConfigDict.TryGetValue(fullTestName, out var genderConfig))
                {
                    config = genderConfig;
                }
                else
                {
                    // Nếu không có config theo giới tính, tìm config chung
                    var genericName = flaggingConfigDict.Keys
                        .FirstOrDefault(k => k.Contains(testType, StringComparison.OrdinalIgnoreCase) 
                                          && !k.Contains("Male") 
                                          && !k.Contains("Female"));
                    
                    if (genericName != null)
                        config = flaggingConfigDict[genericName];
                }

                // Generate random value dựa trên thresholds từ FlaggingSetConfig
                double value;
                if (config != null && config.LowThreshold.HasValue && config.HighThreshold.HasValue)
                {
                    // Sử dụng hàm GenerateRandomTestValue với phân bổ:
                    // 10% Low, 80% Normal, 10% High
                    value = GenerateRandomTestValue(config.LowThreshold.Value, config.HighThreshold.Value);
                }
                else
                {
                    // Fallback: Default values nếu không tìm thấy config
                    value = testType switch
                    {
                        "WBC" => _random.Next(4000, 10000),
                        "RBC" => Math.Round(_random.NextDouble() * (6.1 - 4.2) + 4.2, 2),
                        "HGB" => Math.Round(_random.NextDouble() * (18 - 12) + 12, 1),
                        "HCT" => Math.Round(_random.NextDouble() * (52 - 37) + 37, 1),
                        "PLT" => _random.Next(150000, 350000),
                        "MCV" => Math.Round(_random.NextDouble() * (100 - 80) + 80, 1),
                        "MCH" => Math.Round(_random.NextDouble() * (33 - 27) + 27, 1),
                        "MCHC" => Math.Round(_random.NextDouble() * (36 - 32) + 32, 1),
                        _ => 0
                    };
                }

                // Tự động tính flag dựa trên value và thresholds
                string flag = CalculateFlagByDistribution(
                    (float)value, 
                    config?.LowThreshold, 
                    config?.HighThreshold
                );

                // Tạo reference range string cho hiển thị
                string? referenceRange = null;
                if (config != null && config.LowThreshold.HasValue && config.HighThreshold.HasValue)
                {
                    referenceRange = $"{config.LowThreshold.Value} - {config.HighThreshold.Value}";
                }

                // Tạo TestResultDetail
                var detail = new TestResultDetail
                {
                    TestResultDetailId = Guid.NewGuid(),
                    ResultId = testResult.ResultId,
                    Type = testType,
                    Value = value,
                    Flag = flag,
                    ReferenceRange = referenceRange
                };

                await _testResultDetailRepository.AddTestResultDetailAsync(detail);
            }

            // ⭐ AUTO COMPLETE TEST ORDER
            if (!string.Equals(testOrder.Status, "Complete", StringComparison.OrdinalIgnoreCase))
            {
                testOrder.Status = "Complete";

                testOrder.RunBy = string.IsNullOrWhiteSpace(request.EnteredBy)
                    ? "System"
                    : request.EnteredBy;

                if (!testOrder.RunOn.HasValue)
                    testOrder.RunOn = DateTime.UtcNow;

                await _testOrderRepository.SaveChangeAsync();
            }
        }

        /// <summary>
        /// Lấy tên đầy đủ của test từ database dựa trên type và giới tính
        /// </summary>
        private string GetFullTestName(string testType, string? gender)
        {
            // Xác định suffix theo giới tính (cho các test phụ thuộc giới tính)
            var genderSuffix = gender?.ToLower() == "male" ? " (Male)" : 
                              gender?.ToLower() == "female" ? " (Female)" : "";

            return testType switch
            {
                "WBC" => "White Blood Cell Count (WBC)",
                "RBC" => $"Red Blood Cell Count (RBC){genderSuffix}",
                "HGB" => $"Hemoglobin (Hb/HGB){genderSuffix}",
                "HCT" => $"Hematocrit (HCT){genderSuffix}",
                "PLT" => "Platelet Count (PLT)",
                "MCV" => "Mean Corpuscular Volume (MCV)",
                "MCH" => "Mean Corpuscular Hemoglobin (MCH)",
                "MCHC" => "Mean Corpuscular Hemoglobin Concentration (MCHC)",
                _ => testType
            };
        }

        /// <summary>
        /// Generate giá trị random với phân bố:
        /// - 10% cơ hội nằm DƯỚI low threshold (sẽ được flag là "Low")
        /// - 80% cơ hội nằm TRONG khoảng bình thường (sẽ được flag là "Normal")
        /// - 10% cơ hội nằm TRÊN high threshold (sẽ được flag là "High")
        /// </summary>
        private double GenerateRandomTestValue(float low, float high)
        {
            if (low >= high)
                throw new ArgumentException("Low threshold must be less than high threshold");

            double probability = _random.NextDouble();
            double range = high - low;

            if (probability < 0.10) // 10% Low
            {
                // Generate value dưới low threshold
                double lowerBound = low - (range * 0.5);
                return Math.Round(lowerBound + _random.NextDouble() * (low - lowerBound), 2);
            }
            else if (probability < 0.90) // 80% Normal
            {
                // Generate value trong khoảng [low, high]
                return Math.Round(low + _random.NextDouble() * range, 2);
            }
            else // 10% High
            {
                // Generate value trên high threshold
                double upperBound = high + (range * 0.5);
                return Math.Round(high + _random.NextDouble() * (upperBound - high), 2);
            }
        }

        /// <summary>
        /// Tự động tính flag dựa trên value so với low và high thresholds
        /// </summary>
        private string CalculateFlagByDistribution(float value, float? low, float? high)
        {
            if (!low.HasValue && !high.HasValue)
                return "Normal";

            if (!high.HasValue)
                return value < low.Value ? "Low" : "Normal";

            if (!low.HasValue)
                return value > high.Value ? "High" : "Normal";

            if (value < low.Value) return "Low";
            if (value > high.Value) return "High";

            return "Normal";
        }
    }
}
