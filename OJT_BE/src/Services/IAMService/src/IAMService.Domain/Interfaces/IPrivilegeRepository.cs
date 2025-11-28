using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Domain.Interfaces
{
    /// <summary>
    /// Interface for privilege repository
    /// </summary>
    public interface IPrivilegeRepository
    {
        /// <summary>
        /// Gets all privileges asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<Privilege>> GetAllPrivilegesAsync(CancellationToken cancellationToken);
        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="privId">The priv identifier.</param>
        /// <returns></returns>
        Task<Privilege> GetByIdAsync(int privId);
    }
}
