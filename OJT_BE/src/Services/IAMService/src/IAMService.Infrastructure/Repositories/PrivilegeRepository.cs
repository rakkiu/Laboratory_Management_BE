using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Infrastructure.Repositories
{
    /// <summary>
    /// privilege repository
    /// </summary>
    /// <seealso cref="IAMService.Domain.Interfaces.IPrivilegeRepository" />
    public class PrivilegeRepository : IPrivilegeRepository
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly IAMDbContext _dbContext;
        /// <summary>
        /// Initializes a new instance of the <see cref="PrivilegeRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public PrivilegeRepository(IAMDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets all privileges asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<Privilege>> GetAllPrivilegesAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Privileges.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="privId">The priv identifier.</param>
        /// <returns></returns>
        public async Task<Privilege> GetByIdAsync(int privId)
        {
            return await _dbContext.Privileges.FindAsync(privId);
        }
    }
}
