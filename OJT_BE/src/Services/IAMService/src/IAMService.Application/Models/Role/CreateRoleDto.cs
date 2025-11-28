using IAMService.Application.Models.Privilege;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Application.Models.Role
{
    public class CreateRoleDto
    {
        public string RoleName { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;
        public List<PrivilegeDto> Privileges { get; set; } = [];
    }
}
