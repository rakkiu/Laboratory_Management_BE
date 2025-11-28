using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Domain.Entities.Patient
{
    public class PatientRecordAuditLog
    {
        public Guid AuditId { get; set; }
        public Guid RecordId { get; set; }
        public string Action { get; set; } = null!;
        public Guid PerformedBy { get; set; }
        public DateTime Timestamp { get; set; }
        public string? ChangedFields { get; set; }  // JSON
        public string? OldValues { get; set; }      // JSON
        public string? NewValues { get; set; }      // JSON

        // Navigation
        public PatientMedicalRecord? Record { get; set; }
    }
}
