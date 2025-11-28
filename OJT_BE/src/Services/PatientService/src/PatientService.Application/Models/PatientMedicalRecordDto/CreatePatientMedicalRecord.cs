using PatientService.Application.Models.PatientDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Models.PatientMedicalRecordDto
{
    /// <summary>
    /// create patient medical record dto
    /// </summary>
    public class CreatePatientMedicalRecord
    {
        /// <summary>
        /// Gets or sets the patient.
        /// </summary>
        /// <value>
        /// The patient.
        /// </value>
        public CreatePatient Patient { get; set; } = null!;
        /// <summary>
        /// Gets or sets the record identifier.
        /// </summary>
        /// <value>
        /// The record identifier.
        /// </value>
        public Guid RecordId { get; set; }
        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        /// <value>
        /// The patient identifier.
        /// </value>
        public Guid PatientId { get; set; }
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        public Guid CreatedBy { get; set; }
        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Gets or sets the updated by.
        /// </summary>
        /// <value>
        /// The updated by.
        /// </value>
        public Guid? UpdatedBy { get; set; }
        /// <summary>
        /// Gets or sets the updated at.
        /// </summary>
        /// <value>
        /// The updated at.
        /// </value>
        public DateTime? UpdatedAt { get; set; }
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public int Version { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleted { get; set; }
    }
}
