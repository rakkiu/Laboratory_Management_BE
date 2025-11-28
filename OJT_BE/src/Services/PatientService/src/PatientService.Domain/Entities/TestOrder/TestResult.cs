using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PatientService.Domain.Entities.TestOrder
{
    public class TestResult
    {
        public Guid ResultId { get; set; }
        public Guid TestOrderId { get; set; }
        public string TestName { get; set; } = null!;
        public string? Interpretation { get; set; }
        public string? InstrumentUsed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public TestOrder TestOrder { get; set; } = null!;
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<TestResultDetail>? TestResultDetails { get; set; }
    }
}
