using System;
using System.Collections.Generic;

namespace PatientService.Application.Models.TestOrderReportDto
{
    public class TestOrderPdfDto
    {
        public Guid Id { get; set; }
        public string PatientName { get; set; } = null!;
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string Status { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public string? RunBy { get; set; }
        public DateTime? RunOn { get; set; }

        public string? ReviewedBy { get; set; }

        public string? InstrumentUsed { get; set; }

        // Bảng kết quả & ghi chú
        public List<TestResultDto>? Results { get; set; }
        public List<string>? Comments { get; set; }
    }
}
