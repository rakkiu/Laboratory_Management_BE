using Microsoft.EntityFrameworkCore;
using PatientService.Application.Interfaces;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Infrastructure.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="PatientService.Domain.Interfaces.IFlaggingSetRepository" />
    public class FlaggingSetRepository : IFlaggingSetRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly IApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlaggingSetRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public FlaggingSetRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<FlaggingSetConfig> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            // Dùng Set<T>() thay vì .FlaggingSetConfigs
            return await _context.Set<FlaggingSetConfig>().FindAsync(new object[] { id }, cancellationToken);
        }

        /// <summary>
        /// Gets the by test name asynchronous.
        /// </summary>
        /// <param name="testName">Name of the test.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<FlaggingSetConfig> GetByTestNameAsync(string testName, CancellationToken cancellationToken = default)
        {
            // Dùng Set<T>() thay vì .FlaggingSetConfigs
            return await _context.Set<FlaggingSetConfig>().FirstOrDefaultAsync(c => c.TestName == testName, cancellationToken);
        }

        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<IEnumerable<FlaggingSetConfig>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<FlaggingSetConfig>().ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="flaggingSetConfig">The flagging set configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task AddAsync(FlaggingSetConfig flaggingSetConfig, CancellationToken cancellationToken = default)
        {
            // Dùng Set<T>() thay vì .FlaggingSetConfigs
            await _context.Set<FlaggingSetConfig>().AddAsync(flaggingSetConfig, cancellationToken);
        }

        /// <summary>
        /// Updates the specified flagging set configuration.
        /// </summary>
        /// <param name="flaggingSetConfig">The flagging set configuration.</param>
        public void Update(FlaggingSetConfig flaggingSetConfig)
        {
            // Dùng Set<T>() thay vì .FlaggingSetConfigs
            _context.Set<FlaggingSetConfig>().Update(flaggingSetConfig);
        }
    }
}