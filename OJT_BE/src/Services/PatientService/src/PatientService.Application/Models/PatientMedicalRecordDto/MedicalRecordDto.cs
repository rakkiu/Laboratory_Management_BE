using PatientService.Application.Models.PatientDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Models.PatientMedicalRecordDto
{
    /// <summary>
    /// medical record dto
    /// </summary>
    public class MedicalRecordDto
    {
        /// <summary>
        /// Gets or sets the record identifier.
        /// </summary>
        public Guid RecordId { get; set; }
        
        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        public Guid PatientId { get; set; }
        
        /// <summary>
        /// Gets or sets the test order identifier.
        /// </summary>
        public Guid TestOrderId { get; set; }
        
        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Gets or sets the updated by.
        /// </summary>
        public Guid? UpdatedBy { get; set; }
        
        /// <summary>
        /// Gets or sets the updated at.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public int Version { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }
        
        /// <summary>
        /// Gets or sets the patient.
        /// </summary>
        public CreatePatient? Patient { get; set; }
    }
}
