using IAMService.Application.Models.Privilege;
using IAMService.Application.Models.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Application.UseCases.Role.Commands.CreateRole
{
    /// <summary>
    /// handler for creating a new role
    /// </summary>
    /// <seealso cref="IRequestHandler&lt;CreateRoleCommand, CreateRoleDto&gt;" />
    /// <seealso cref="IRequestHandler&lt;CreateRoleCommand, CreateRoleDto&gt;" />
    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, CreateRoleDto>
    {
        /// <summary>
        /// The role repository
        /// </summary>
        private readonly IRoleRepository _roleRepository;
        /// <summary>
        /// The privilege repository
        /// </summary>
        private readonly IPrivilegeRepository _privilegeRepository;
        /// <summary>
        /// The role privilege repository
        /// </summary>
        private readonly IRolePrivilegeRepository _rolePrivilegeRepository;
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRoleHandler" /> class.
        /// </summary>
        /// <param name="roleRepository">The role repository.</param>
        /// <param name="privilegeRepository">The privilege repository.</param>
        /// <param name="rolePrivilegeRepository">The role privilege repository.</param>
        public CreateRoleHandler(IRoleRepository roleRepository, IPrivilegeRepository privilegeRepository, IRolePrivilegeRepository rolePrivilegeRepository)
        {
            _roleRepository = roleRepository;
            _privilegeRepository = privilegeRepository;
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
        /// <exception cref="ArgumentException">Role with RoleCode '{request.RoleCode}' already exists.
        /// or
        /// Privilege with Id '{privId}' does not exist.</exception>
        public async Task<CreateRoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {

            if(await _roleRepository.RoleExistsAsync(request.RoleCode))
            {
                throw new ArgumentException($"Role with RoleCode '{request.RoleCode}' already exists.");
            }
            // Map request to Role entity
            var role = new Domain.Entities.Role
            {
                RoleName = request.RoleName,
                RoleCode = request.RoleCode,
                RoleDescription = request.RoleDescription
            };
            // Save role to repository
            await _roleRepository.AddAsync(role);
            foreach (var priv in request.Privileges)
            {
                var privId = priv.PrivilegeId;
                var privilege = await _privilegeRepository.GetByIdAsync(privId);
                if (privilege == null)
                {
                    throw new ArgumentException($"Privilege with Id '{privId}' does not exist.");
                }
                var rolePrivilege = new RolePrivilege
                {
                    RoleCode = role.RoleCode,
                    PrivilegeId = privilege.PrivilegeId
                };
                await _rolePrivilegeRepository.AddAsync(rolePrivilege);
                role.RolePrivileges.Add(rolePrivilege);
            }
            // Map saved role to CreateRoleDto
            var result = new CreateRoleDto
            {
                RoleName = role.RoleName,
                RoleCode = role.RoleCode,
                RoleDescription = role.RoleDescription,
                Privileges = role.RolePrivileges.Select(rp => new PrivilegeDto
                {
                    PrivilegeId = rp.Privilege.PrivilegeId,
                    PrivilegeName = rp.Privilege.PrivilegeName,
                    Description = rp.Privilege.Description
                }).ToList()
            };
            return result;
        }
    }
}
