using IAMService.Application.Models.Privilege;
using IAMService.Application.Models.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Application.UseCases.Role.Commands.UpdateRole
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAMService.Application.UseCases.Role.Commands.UpdateRole.UpdateRoleCommand, IAMService.Application.Models.Role.UpdateRoleDto&gt;" />
    public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, UpdateRoleDto>
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
        /// Initializes a new instance of the <see cref="UpdateRoleHandler"/> class.
        /// </summary>
        /// <param name="roleRepository">The role repository.</param>
        /// <param name="rolePrivilegeRepository">The role privilege repository.</param>
        public UpdateRoleHandler(IRoleRepository roleRepository, IRolePrivilegeRepository rolePrivilegeRepository)
        {
            _roleRepository = roleRepository;
            _rolePrivilegeRepository = rolePrivilegeRepository;
        }
        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Role with ID {request.RoleCode} not found.</exception>
        public async Task<UpdateRoleDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleCode, cancellationToken);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {request.RoleCode} not found.");
            }
            role.RoleName = request.RoleName;
            role.RoleDescription = request.RoleDescription;
            await _roleRepository.UpdateAsync(role, cancellationToken);
            await _rolePrivilegeRepository.DeleteByRoleCode(request.RoleCode);

            var privilegeDtos = new List<PrivilegeDto>();
            foreach (var rp in request.Privileges)
            {
                privilegeDtos.Add(new PrivilegeDto
                {
                    PrivilegeId = rp.PrivilegeId,
                    PrivilegeName = rp.PrivilegeName,
                    Description = rp.Description
                });
                await _rolePrivilegeRepository.AddAsync(new RolePrivilege
                {
                    RoleCode = request.RoleCode,
                    PrivilegeId = rp.PrivilegeId
                });
            }

            return new UpdateRoleDto
            {
                RoleCode = role.RoleCode,
                RoleName = role.RoleName,
                RoleDescription = role.RoleDescription,
                Privileges = privilegeDtos
            };
        }
    }
}
