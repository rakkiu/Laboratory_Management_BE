using System;

namespace PatientService.Application.Models.TestOrderDto
{
    public class TestOrderListDto
    {
        public Guid TestOrderId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? RunBy { get; set; }
        public DateTime? RunOn { get; set; }
    }
}
