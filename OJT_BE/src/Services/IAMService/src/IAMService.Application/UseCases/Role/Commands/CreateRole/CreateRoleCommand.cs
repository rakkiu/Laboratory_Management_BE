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
    /// command for creating a new role
    /// </summary>
    /// <seealso cref="IRequest&lt;CreateRoleDto&gt;" />
    /// <seealso cref="IBaseRequest" />
    /// <seealso cref="IEquatable&lt;CreateRoleCommand&gt;" />
    public record CreateRoleCommand : IRequest<CreateRoleDto>
    {
        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>
        /// The name of the role.
        /// </value>
        public string RoleName { get; set; }
        /// <summary>
        /// Gets or sets the role code.
        /// </summary>
        /// <value>
        /// The role code.
        /// </value>
        public string RoleCode { get; set; }
        /// <summary>
        /// Gets or sets the role description.
        /// </summary>
        /// <value>
        /// The role description.
        /// </value>
        public string RoleDescription { get; set; }
        /// <summary>
        /// Gets or sets the privileges.
        /// </summary>
        /// <value>
        /// The privileges.
        /// </value>
        public List<PrivilegeDto> Privileges { get; set; }
    }
}
