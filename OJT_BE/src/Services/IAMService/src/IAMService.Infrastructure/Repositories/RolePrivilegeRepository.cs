using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Infrastructure.Repositories
{
    /// <summary>
    /// role privilege repository
    /// </summary>
    /// <seealso cref="IAMService.Domain.Interfaces.IRolePrivilegeRepository" />
    public class RolePrivilegeRepository : IRolePrivilegeRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly IAMDbContext _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="RolePrivilegeRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public RolePrivilegeRepository(IAMDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="rolePrivilege">The role privilege.</param>
        public async Task AddAsync(RolePrivilege rolePrivilege)
        {
            await _context.RolePrivileges.AddAsync(rolePrivilege);
            _context.SaveChanges();
        }

        public async Task DeleteByRoleCode(string roleCode)
        {
            _context.RolePrivileges.RemoveRange(_context.RolePrivileges.Where(rp => rp.RoleCode == roleCode));
            await _context.SaveChangesAsync();
        }
    }
}
