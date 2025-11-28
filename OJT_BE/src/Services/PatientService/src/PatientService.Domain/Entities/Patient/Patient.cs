using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Domain.Entities.Patient
{
    public class Patient
    {
        public Guid PatientId { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastTestDate { get; set; }
        public string IdentifyNumber { get; set; } = null!;
        public Guid? UserId { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation
        public ICollection<PatientMedicalRecord>? MedicalRecords { get; set; }
    }
}
