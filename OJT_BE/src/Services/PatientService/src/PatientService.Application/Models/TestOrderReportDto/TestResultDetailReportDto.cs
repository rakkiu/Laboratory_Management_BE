public class TestResultDetailReportDto
{
    public string Type { get; set; } = null!;
    public double Value { get; set; }
    public string? ReferenceRange { get; set; }
    public string? Flag { get; set; }
}
