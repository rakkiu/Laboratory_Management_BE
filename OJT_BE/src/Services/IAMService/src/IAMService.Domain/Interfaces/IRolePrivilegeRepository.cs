using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Domain.Interfaces
{
    /// <summary>
    /// Interface for role privilege repository
    /// </summary>
    public interface IRolePrivilegeRepository
    {
        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="rolePrivilege">The role privilege.</param>
        /// <returns></returns>
        Task AddAsync(RolePrivilege rolePrivilege);
        Task DeleteByRoleCode(string roleCode);
    }
}
