using PatientService.Domain.Entities.TestOrder;
using System.Threading.Tasks;

namespace PatientService.Domain.Interfaces
{
    /// <summary>
    /// Defines the contract for a repository that handles <see cref="TestOrderAuditLog"/> operations.
    /// </summary>
    public interface IAuditLogRepository
    {
        /// <summary>
        /// Asynchronously adds a new <see cref="TestOrderAuditLog"/> entry.
        /// </summary>
        /// <param name="log">The audit log entry to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddLogAsync(TestOrderAuditLog log);
    }
}