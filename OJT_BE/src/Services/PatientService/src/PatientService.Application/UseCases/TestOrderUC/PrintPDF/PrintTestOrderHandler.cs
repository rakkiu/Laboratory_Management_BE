using PatientService.Application.Interfaces;
using PatientService.Application.Models.TestOrderReportDto;
using PatientService.Domain.Interfaces.TestOrderService;


namespace PatientService.Application.UseCases.TestOrderUC.PrintPDF
{
    public class PrintTestOrderHandler : IRequestHandler<PrintTestOrderQuery, PrintFileResult>
    {
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IPdfService _pdfService;
        private readonly IIamUserClient _iamUserClient;

        public PrintTestOrderHandler(
            ITestOrderRepository testOrderRepository,
            IPdfService pdfService,
            IIamUserClient iamUserClient)
        {
            _testOrderRepository = testOrderRepository;
            _pdfService = pdfService;
            _iamUserClient = iamUserClient;
        }

        public async Task<PrintFileResult> Handle(PrintTestOrderQuery request, CancellationToken cancellationToken)
        {
            var testOrder = await _testOrderRepository.GetDetailAsync(request.TestOrderId, cancellationToken);

            if (testOrder == null)
                throw new KeyNotFoundException($"Test order with ID {request.TestOrderId} not found.");

            // Allow printing for Completed status (case-insensitive)
            if (!string.Equals(testOrder.Status, "Complete", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Cannot print PDF. Test order status is '{testOrder.Status}'. Only 'Completed' status is allowed.");

            // --- Resolve CreatedBy GUID to full name via IAM ---
            string? createdByName = null;
            if (!string.IsNullOrWhiteSpace(testOrder.CreatedBy) && Guid.TryParse(testOrder.CreatedBy, out var createdByGuid))
            {
                createdByName = await _iamUserClient.GetUserFullNameAsync(createdByGuid, cancellationToken);
            }

            var createdByDisplay = !string.IsNullOrWhiteSpace(createdByName)
                ? createdByName
                : (!string.IsNullOrWhiteSpace(testOrder.CreatedBy) ? testOrder.CreatedBy : "Unknown");

            var dto = new TestOrderPdfDto
            {
                Id = testOrder.TestOrderId,
                PatientName = testOrder.PatientName,
                Gender = testOrder.Gender,
                DateOfBirth = testOrder.DateOfBirth,
                PhoneNumber = testOrder.PhoneNumber,
                Status = testOrder.Status,
                CreatedBy = createdByDisplay,
                CreatedOn = testOrder.CreatedAt,
                RunBy = testOrder.RunBy,
                RunOn = testOrder.RunOn,
                ReviewedBy = testOrder.ReviewedBy,

                InstrumentUsed = testOrder.TestResults?.FirstOrDefault()?.InstrumentUsed,  // 🔥 FIXED

                Results = testOrder.TestResults?.Select(r => new TestResultDto
                {
                    TestName = r.TestName,
                    InstrumentUsed = r.InstrumentUsed,

                    Details = r.TestResultDetails!.Select(d => new TestResultDetailReportDto
                    {
                        Type = d.Type,
                        Value = d.Value,
                        ReferenceRange = d.ReferenceRange,
                        Flag = d.Flag
                    }).ToList()

                }).ToList(),


                Comments = testOrder.Comments?.Select(c => c.Content).ToList()
            };

            string SafeFileName(string name)
            {
                var invalidChars = Path.GetInvalidFileNameChars();
                return new string(name
                    .Replace(" ", "_")
                    .Where(c => !invalidChars.Contains(c))
                    .ToArray());
            }

            var patientName = SafeFileName(testOrder.PatientName ?? "Unknown");
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");

            var fileName = !string.IsNullOrWhiteSpace(request.FileName)
                ? SafeFileName(request.FileName) + ".pdf"
                : $"Detail-{patientName}-{datePart}.pdf";

            var bytes = await _pdfService.GenerateTestOrderReportAsync(dto, fileName);

            return new PrintFileResult
            {
                FileBytes = bytes,
                FileName = fileName
            };
        }
    }
}
