using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Application.UseCases.Role.Commands.DeleteRole
{
    /// <summary>
    /// Delete role handler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAMService.Application.UseCases.Role.Commands.DeleteRole.DeleteRoleCommand&gt;" />
    public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand>
    {
        /// <summary>
        /// The role repository
        /// </summary>
        private readonly IRoleRepository _roleRepository;
        /// <summary>
        /// The role privilege repository
        /// </summary>
        private readonly IRolePrivilegeRepository _rolePrivilegeRepository;
        /// <summary>
        /// The user repository
        /// </summary>
        private readonly IUserRepository _userRepository;
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteRoleHandler"/> class.
        /// </summary>
        /// <param name="roleRepository">The role repository.</param>
        /// <param name="rolePrivilegeRepository">The role privilege repository.</param>
        /// <param name="userRepository">The user repository.</param>
        public DeleteRoleHandler(IRoleRepository roleRepository, IRolePrivilegeRepository rolePrivilegeRepository, IUserRepository userRepository)
        {
            _roleRepository = roleRepository;
            _rolePrivilegeRepository = rolePrivilegeRepository;
            _userRepository = userRepository;
        }
        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <exception cref="System.ArgumentException">Role with code {request.RoleCode} cannot be deleted.</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Role with code {request.RoleCode} does not exist.</exception>
        public async Task Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            List<string> roleCodesCannotDelete = [ "ADMIN", "CUSTOM_ROLE", "LAB_MANAGER", "LAB_USER", "SERVICE" ];
            if (roleCodesCannotDelete.Contains(request.RoleCode))
            {
                throw new ArgumentException($"Role with code {request.RoleCode} cannot be deleted.");
            }
            else
            {
                var role = await _roleRepository.GetByIdAsync(request.RoleCode, cancellationToken) ??
                    throw new KeyNotFoundException($"Role with code {request.RoleCode} does not exist.");
                
                
                List<IAMService.Domain.Entities.User> userWithRoleDetected = await _userRepository.GetUsersByRoleCodeAsync(request.RoleCode, cancellationToken);
                if(userWithRoleDetected.Any())
                {
                    foreach(var user in userWithRoleDetected)
                    {
                        user.RoleCode = "CUSTOM_ROLE";
                        _userRepository.UpdateV1(user);
                    }
                }
                await _rolePrivilegeRepository.DeleteByRoleCode(request.RoleCode);
                await _roleRepository.DeleteAsync(role, cancellationToken);
            }
        }
    }
}
