namespace PatientService.Application.Models.TestOrderReportDto
{
    public class TestOrderEmailDto
    {
        public Guid TestOrderId { get; set; }
        public string PatientName { get; set; } = null!;
        public string? Email { get; set; }
    }
}
