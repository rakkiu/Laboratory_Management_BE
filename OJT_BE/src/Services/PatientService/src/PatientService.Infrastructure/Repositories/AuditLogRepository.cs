using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;
using PatientService.Infrastructure.Data;
using System.Threading.Tasks;

namespace PatientService.Infrastructure.Repositories
{
    /// <summary>
    /// Implements the <see cref="IAuditLogRepository"/> interface to manage audit log entries
    /// in the database.
    /// </summary>
    public class AuditLogRepository : IAuditLogRepository
    {
        /// <summary>
        /// The database context for interacting with patient data.
        /// </summary>
        private readonly PatientDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogRepository"/> class.
        /// </summary>
        /// <param name="context">The <see cref="PatientDbContext"/> injected by DI.</param>
        public AuditLogRepository(PatientDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Asynchronously adds a new <see cref="TestOrderAuditLog"/> entry to the database.
        /// </summary>
        /// <param name="log">The audit log entry to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task AddLogAsync(TestOrderAuditLog log)
        {
            // Because LogId is 'int', EF Core automatically handles it as an Identity (auto-increment) column.
            await _context.TestOrderAuditLogs.AddAsync(log);
        }
    }
}