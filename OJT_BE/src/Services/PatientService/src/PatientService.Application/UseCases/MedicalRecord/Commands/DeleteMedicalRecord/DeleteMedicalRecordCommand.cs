using PatientService.Application.Interfaces;
namespace PatientService.Application.UseCases.MedicalRecord.Commands.DeleteMedicalRecord
{
    /// <summary>
    /// delete medical record command
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Boolean&gt;" />
    /// <seealso cref="MediatR.IRequest&lt;System.Boolean&gt;" />
    /// <seealso cref="PatientService.Application.Interfaces.IAuditableCommand" />
    public class DeleteMedicalRecordCommand : IRequest<bool>, IAuditableCommand
    {
        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        /// <value>
        /// The patient identifier.
        /// </value>
        public Guid PatientId { get; set; }
        /// <summary>
        /// Gets or sets the deleted by.
        /// </summary>
        /// <value>
        /// The deleted by.
        /// </value>
        public Guid DeletedBy { get; set; }

        // IAuditableCommand implementation
        /// <summary>
        /// User who performed the action
        /// </summary>
        public Guid PerformedBy => DeletedBy;
        /// <summary>
        /// Action type for audit log (CREATE, UPDATE, DELETE)
        /// </summary>
        /// <returns></returns>
        public string GetAuditAction() => "DELETE";
        /// <summary>
        /// Get the entity ID that is being audited
        /// </summary>
        /// <returns></returns>
        public Guid? GetEntityId() => PatientId;
    }
}