using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Infrastructure.Repositories
{
    /// <summary>
    /// repository for roles
    /// </summary>
    /// <seealso cref="IAMService.Domain.Interfaces.IRoleRepository" />
    public class RoleRepository : IRoleRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly IAMDbContext _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleRepository" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public RoleRepository(IAMDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="role">The role.</param>
        public async Task AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            _context.SaveChanges();
        }

        public Task DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            _context.Roles.Remove(role);
            return _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Gets all roles asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Role>> GetAllRolesAsync(CancellationToken cancellationToken)
        {
            return await _context.Roles
                .Include(r => r.RolePrivileges)
                    .ThenInclude(rp => rp.Privilege)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="roleCode">The role code.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<Role> GetByIdAsync(string roleCode, CancellationToken cancellationToken)
        {
            return await _context.Roles
                .Include(r => r.RolePrivileges)
                    .ThenInclude(rp => rp.Privilege)
                    .ThenInclude(p => p.RolePrivileges)
                .FirstOrDefaultAsync(r => r.RoleCode == roleCode, cancellationToken)!;
        }

        /// <summary>
        /// Roles the exists asynchronous.
        /// </summary>
        /// <param name="RoleCode">The role code.</param>
        /// <returns></returns>
        public async Task<bool> RoleExistsAsync(string RoleCode)
        {
            var loadedRole = await _context.Roles
                .FindAsync(RoleCode);
            if (loadedRole is null)
                return false;
            return true;
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            _context.Update(role);
            await _context.SaveChangesAsync(cancellationToken); 
        }
    }
}
