using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Domain.Entities.Patient
{
    public class PatientMedicalRecord
    {
        public Guid RecordId { get; set; }
        public Guid PatientId { get; set; }
        //public string? ClinicalNotes { get; set; }
        //public string? Diagnosis { get; set; }
        //public Guid DoctorId { get; set; }
        //public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Version { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation
        public Patient? Patient { get; set; }
        public ICollection<PatientRecordAuditLog>? AuditLogs { get; set; }
    }
}
