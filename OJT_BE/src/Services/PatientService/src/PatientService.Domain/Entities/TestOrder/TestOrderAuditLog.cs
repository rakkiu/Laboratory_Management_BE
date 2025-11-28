using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Domain.Entities.TestOrder
{
    public class TestOrderAuditLog
    {
        public int LogId { get; set; }
        public Guid TestOrderId { get; set; }
        public string? UserId { get; set; } // Text field - không FK thật
        public string ActionType { get; set; } = null!;
        public string? ChangedFields { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
