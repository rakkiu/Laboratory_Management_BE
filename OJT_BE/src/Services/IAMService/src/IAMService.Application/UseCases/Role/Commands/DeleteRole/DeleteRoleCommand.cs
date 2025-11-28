using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Application.UseCases.Role.Commands.DeleteRole
{
    /// <summary>
    /// Delete role command
    /// </summary>
    /// <seealso cref="MediatR.IRequest" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;IAMService.Application.UseCases.Role.Commands.DeleteRole.DeleteRoleCommand&gt;" />
    public record DeleteRoleCommand(string RoleCode) :IRequest;
}
