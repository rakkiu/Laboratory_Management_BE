using IAMService.Application.Models.Privilege;
using IAMService.Application.Models.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Application.UseCases.Role.Queries.GetAllRole
{
    public class GetAllRolesHandler : IRequestHandler<GetAllRolesQuery, List<CreateRoleDto>>
    {
        private readonly IRoleRepository _roleRepository;
        public GetAllRolesHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<List<CreateRoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.GetAllRolesAsync(cancellationToken);
            var roleDTOs = roles.Select(role => new CreateRoleDto
            {
                RoleCode = role.RoleCode,
                RoleName = role.RoleName,
                RoleDescription = role.RoleDescription,
                Privileges = role.RolePrivileges?.Select(rp => new PrivilegeDto
                {
                    PrivilegeId = rp.Privilege.PrivilegeId,
                    PrivilegeName = rp.Privilege.PrivilegeName,
                    Description = rp.Privilege.Description
                }).ToList() ?? new List<PrivilegeDto>()
            });
            return roleDTOs.ToList();
        }
    }
}
