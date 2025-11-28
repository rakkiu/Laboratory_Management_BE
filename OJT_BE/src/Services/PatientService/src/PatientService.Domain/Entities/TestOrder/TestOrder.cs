using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PatientService.Domain.Entities.TestOrder
{
    public class TestOrder
    {
        public Guid RecordId { get; set; }
        public Guid TestOrderId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string Status { get; set; } = "Pending";
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? RunBy { get; set; }
        public DateTime? RunOn { get; set; }
        public string? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation
        public ICollection<TestResult>? TestResults { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }

}
