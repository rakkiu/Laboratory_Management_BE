using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Domain.Interfaces
{
    /// <summary>
    /// Interface for role repository
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        Task AddAsync(Role role);
        Task DeleteAsync(Role role, CancellationToken cancellationToken);
        Task<IEnumerable<Role>> GetAllRolesAsync(CancellationToken cancellationToken);
        Task<Role> GetByIdAsync(string roleCode, CancellationToken cancellationToken);

        /// <summary>
        /// Roles the exists asynchronous.
        /// </summary>
        /// <param name="RoleCode">The role code.</param>
        /// <returns></returns>
        Task<bool> RoleExistsAsync(string RoleCode);
        Task UpdateAsync(Role role, CancellationToken cancellationToken);
    }
}
