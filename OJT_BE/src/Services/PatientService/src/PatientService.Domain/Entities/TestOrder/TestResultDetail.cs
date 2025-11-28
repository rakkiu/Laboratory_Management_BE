using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Domain.Entities.TestOrder
{
    public class TestResultDetail
    {
        public Guid TestResultDetailId { get; set; }
        public Guid ResultId { get; set; }
        public string Type { get; set; } = null!;
        public double Value { get; set; }
        public string Flag { get; set; } = "Normal";
        public string? ReferenceRange { get; set; }

        // Navigation
        public TestResult TestResult { get; set; } = null!;
    }
}
