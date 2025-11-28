using PatientService.Application.Interfaces;
using PatientService.Application.Models.PatientDto;
using PatientService.Application.Models.PatientMedicalRecordDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.MedicalRecord.Commands.CreateMedicalRecord
{
    /// <summary>
    /// create patient medical record command
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;PatientService.Application.Models.PatientMedicalRecordDto.CreatePatientMedicalRecord&gt;" />
    /// <seealso cref="PatientService.Application.Interfaces.IAuditableCommand" />
    public class CreatePatientMedicalRecordCommad : IRequest<CreatePatientMedicalRecord>, IAuditableCommand
    {
        /// <summary>
        /// Gets or sets the patient.
        /// </summary>
        /// <value>
        /// The patient.
        /// </value>
        public CreatePatient Patient { get; set; } = null!;
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        public Guid CreatedBy { get; set; }

        // IAuditableCommand implementation
        /// <summary>
        /// User who performed the action
        /// </summary>
        public Guid PerformedBy => CreatedBy;

        /// <summary>
        /// Action type for audit log (CREATE, UPDATE, DELETE)
        /// </summary>
        /// <returns></returns>
        public string GetAuditAction() => "CREATE";

        /// <summary>
        /// Get the entity ID that is being audited
        /// </summary>
        /// <returns></returns>
        public Guid? GetEntityId() => null; // Will be set after creation
    }
}
