using System;

namespace PatientService.Application.Models.TestOrderReportDto
{
    public class TestOrderExportDto
    {
        public Guid Id { get; set; }
        public string PatientName { get; set; } = null!;
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string Status { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public string? RunBy { get; set; }
        public DateTime? RunOn { get; set; }
    }
}
