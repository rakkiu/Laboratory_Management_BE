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
    /// command for updating a role
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;IAMService.Application.Models.Role.UpdateRoleDto&gt;" />
    public class UpdateRoleCommand : IRequest<UpdateRoleDto>
    {
        /// <summary>
        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>
        /// The name of the role.
        /// </value>
        public string RoleName { get; set; } = null!;
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
