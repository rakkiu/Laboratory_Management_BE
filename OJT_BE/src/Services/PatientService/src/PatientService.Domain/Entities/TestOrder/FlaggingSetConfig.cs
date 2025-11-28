using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Domain.Entities.TestOrder
{
    public class FlaggingSetConfig
    {
        public int ConfigId { get; set; }
        public string TestName { get; set; } = null!;
        public float? LowThreshold { get; set; }
        public float? HighThreshold { get; set; }
        public float? CriticalThreshold { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Version { get; set; } = "1.0";
    }
}
