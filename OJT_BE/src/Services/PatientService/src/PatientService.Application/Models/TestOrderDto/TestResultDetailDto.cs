using System;

namespace PatientService.Application.Models.TestOrderDto
{
    public class TestResultDetailDto
    {
        public Guid ResultId { get; set; }
        public string TestName { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string? ReferenceRange { get; set; }
        public string? Interpretation { get; set; }
        public string? InstrumentUsed { get; set; }
        public string Flag { get; set; } = "Normal";
        public DateTime CreatedAt { get; set; }
    }
}
