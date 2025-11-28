namespace PatientService.Application.Interfaces
{
    /// <summary>
    /// Marker interface for commands that require audit logging
    /// </summary>
    public interface IAuditableCommand
    {
        /// <summary>
        /// User who performed the action
        /// </summary>
        Guid PerformedBy { get; }
        
        /// <summary>
        /// Action type for audit log (CREATE, UPDATE, DELETE)
        /// </summary>
        string GetAuditAction();
        
        /// <summary>
        /// Get the entity ID that is being audited
        /// </summary>
        Guid? GetEntityId();
    }
}
