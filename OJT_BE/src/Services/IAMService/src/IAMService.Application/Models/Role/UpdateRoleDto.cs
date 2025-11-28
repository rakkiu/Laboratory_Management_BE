using IAMService.Application.Models.Privilege;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Application.Models.Role
{
    /// <summary>
    /// Dto for updating a role
    /// </summary>
    public class UpdateRoleDto
    {
        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>
        /// The name of the role.
        /// </value>
        public string RoleName { get; set; } = null!;
        /// <summary>
        /// Gets or sets the role code.
        /// </summary>
        /// <value>
        /// The role code.
        /// </value>
        public string RoleCode { get; set; } = null!;
        /// <summary>
        /// Gets or sets the role description.
        /// </summary>
        /// <value>
        /// The role description.
        /// </value>
        public string RoleDescription { get; set; } = null!;
        /// <summary>
        /// Gets or sets the privilege dtos.
        /// </summary>
        /// <value>
        /// The privilege dtos.
        /// </value>
        public List<PrivilegeDto> Privileges { get; set; } = [];
    }
}
