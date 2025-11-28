public class TestResultDto
{
    public string TestName { get; set; } = null!;
    public string? InstrumentUsed { get; set; }

    public List<TestResultDetailReportDto>? Details { get; set; }
}