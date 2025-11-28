namespace PatientService.Application.UseCases.TestOrderUC.PrintPDF
{
    public class PrintFileResult
    {
        public required byte[] FileBytes { get; set; }
        public required string FileName { get; set; }
    }

    public record PrintTestOrderQuery(Guid TestOrderId, string? FileName = null) : IRequest<PrintFileResult>;
}
