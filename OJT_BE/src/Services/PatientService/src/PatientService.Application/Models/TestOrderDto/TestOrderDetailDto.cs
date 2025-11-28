using System;

namespace PatientService.Application.Models.TestOrderDto
{
    public class TestOrderDetailDto
    {
        public Guid TestOrderId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string Status { get; set; } = "Pending";
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? RunBy { get; set; }
        public DateTime? RunOn { get; set; }
        public string? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }
}
