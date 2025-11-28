using PatientService.Domain.Entities.Patient;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;
using PatientService.Domain.Interfaces.TestOrderService;
using System;
using System.Globalization;

namespace PatientService.Application.UseCases.TestOrderUC.TestOrders.Commands.CreateTestOrder
{
    public class CreateTestOrderHandler : IRequestHandler<CreateTestOrderCommand>
    {
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IPatientMedicalRecordRepository _patientMedicalRecordRepository;
        private readonly IAuditLogRepository _auditLogRepository;  // ✅ ADD

        private static readonly string[] AcceptableDobFormats =
        {
            "MM/dd/yyyy"
        };
        private const int TimeZoneOffsetHours = 7; // UTC+7 (Vietnam)

        public CreateTestOrderHandler(
            ITestOrderRepository testOrderRepository,
            IPatientRepository patientRepository,
            IPatientMedicalRecordRepository patientMedicalRecordRepository,
            IAuditLogRepository auditLogRepository) // ✅ ADD
        {
            _testOrderRepository = testOrderRepository;
            _patientRepository = patientRepository;
            _patientMedicalRecordRepository = patientMedicalRecordRepository;
            _auditLogRepository = auditLogRepository; // ✅ ADD
        }

        public async Task Handle(CreateTestOrderCommand request, CancellationToken cancellationToken)
        {
            // Parse DOB
            if (!DateTime.TryParseExact(request.Patient.DateOfBirth, AcceptableDobFormats,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob))
            {
                throw new ArgumentException("Invalid date of birth. Accepted format: MM/dd/yyyy.");
            }

            dob = DateTime.SpecifyKind(dob.AddHours(TimeZoneOffsetHours), DateTimeKind.Utc);

            DateTime? lastTestDate = null;
            if (!string.IsNullOrWhiteSpace(request.Patient.LastTestDate))
            {
                if (!DateTime.TryParseExact(request.Patient.LastTestDate, AcceptableDobFormats,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var ltd))
                {
                    throw new ArgumentException("Invalid last test date. Accepted format: MM/dd/yyyy.");
                }

                lastTestDate = DateTime.SpecifyKind(ltd.AddHours(TimeZoneOffsetHours), DateTimeKind.Utc);
            }

            var nowUtc = DateTime.UtcNow;
            if (_patientRepository.EmailIsExist(request.Patient.Email))
            {
                                throw new ArgumentException("Email already exists.");
            }
            if(_patientRepository.IdentifyNumberIsExist(request.Patient.IdentifyNumber))
            {
                                throw new ArgumentException("Identify number already exists.");
            }
            if(_patientRepository.PhoneNumberIsExist(request.Patient.PhoneNumber))
            {
                                throw new ArgumentException("Phone number already exists.");
            }

            // === CREATE PATIENT ===
            Patient patient = new()
            {
                PatientId = Guid.NewGuid(),
                Address = request.Patient.Address,
                DateOfBirth = dob,
                CreatedAt = nowUtc,
                Email = request.Patient.Email,
                FullName = request.Patient.FullName,
                Gender = request.Patient.Gender,
                IdentifyNumber = request.Patient.IdentifyNumber,
                PhoneNumber = request.Patient.PhoneNumber,
                IsDeleted = false,
                LastTestDate = lastTestDate,
                UpdatedAt = nowUtc,
                UserId = request.Patient.UserId
            };

            // === CREATE MEDICAL RECORD ===
            PatientMedicalRecord record = new()
            {
                RecordId = Guid.NewGuid(),
                PatientId = patient.PatientId,
                CreatedAt = nowUtc,
                IsDeleted = false,
                Version = 1,
                UpdatedAt = null,
                UpdatedBy = null
            };

            // === CREATE TEST ORDER ===
            TestOrder testOrder = new()
            {
                RecordId = record.RecordId,
                TestOrderId = Guid.NewGuid(),
                PatientId = patient.PatientId,
                PatientName = patient.FullName,
                DateOfBirth = patient.DateOfBirth,
                Status = "Pending",
                Address = patient.Address,
                Age = (int)((DateTime.UtcNow - patient.DateOfBirth).TotalDays / 365.25),
                CreatedAt = nowUtc,
                Comments = null,
                CreatedBy = request.CreatedBy,
                Gender = patient.Gender,
                IsDeleted = false,
                PhoneNumber = patient.PhoneNumber
            };

            // === INSERT DATA ===
            await _patientRepository.AddPatientAsync(patient);
            await _patientMedicalRecordRepository.AddMedicalRecordAsync(record);
            await _testOrderRepository.AddTestOrderAsync(testOrder);

            // === ADD AUDIT LOG ===
            var auditLog = new TestOrderAuditLog
            {
                TestOrderId = testOrder.TestOrderId,
                UserId = request.CreatedBy.ToString(),
                ActionType = "CREATE_TEST_ORDER",
                Timestamp = DateTime.UtcNow,
                ChangedFields =
                    $"Created TestOrder | PatientName: {testOrder.PatientName} | RecordId: {record.RecordId} | Status: {testOrder.Status}"
            };

            await _auditLogRepository.AddLogAsync(auditLog);

            // === SAVE CHANGES ===
            await _testOrderRepository.SaveChangeAsync();
        }
    }
}
