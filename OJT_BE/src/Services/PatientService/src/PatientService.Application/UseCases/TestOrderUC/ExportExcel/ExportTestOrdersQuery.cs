using MediatR;

namespace PatientService.Application.UseCases.TestOrderUC.ExportExcel
{
    public class ExportFileResult
    {
        public required byte[] FileBytes { get; set; }
        public required string FileName { get; set; }
    }

    // Cho phép PatientId có thể null
    public record ExportTestOrdersQuery(Guid? PatientId) : IRequest<ExportFileResult>;
}
