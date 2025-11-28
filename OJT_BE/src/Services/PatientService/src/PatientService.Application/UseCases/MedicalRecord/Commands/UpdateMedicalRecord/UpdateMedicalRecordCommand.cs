using PatientService.Application.Interfaces;
using PatientService.Application.Models.PatientDto;
using PatientService.Application.Models.PatientMedicalRecordDto;
namespace PatientService.Application.UseCases.MedicalRecord.Commands.UpdateMedicalRecord
{
    /// <summary>
    /// update patient medical record command
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;PatientService.Application.Models.PatientMedicalRecordDto.MedicalRecordDto&gt;" />
    /// <seealso cref="MediatR.IRequest&lt;PatientService.Application.Models.PatientMedicalRecordDto.MedicalRecordDto&gt;" />
    /// <seealso cref="PatientService.Application.Interfaces.IAuditableCommand" />
    public class UpdatePatientMedicalRecordCommand : IRequest<MedicalRecordDto>, IAuditableCommand
    {
        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        public Guid PatientId { get; set; }

        /// <summary>
        /// Gets or sets the patient information to update.
        /// </summary>
        public CreatePatient Patient { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Test Order ID for the medical record.
        /// </summary>
      
        public Guid UpdatedBy { get; set; }

        // IAuditableCommand implementation
        public Guid PerformedBy => UpdatedBy;
        public string GetAuditAction() => "UPDATE";
        
        // The entity being audited is the PatientMedicalRecord, which we will find in the handler.
        // We return null here and set it dynamically in the AuditBehavior if needed, 
        // or rely on the ChangeTracker. For now, we can't know the RecordId here.
        public Guid? GetEntityId() => null;
    }
}