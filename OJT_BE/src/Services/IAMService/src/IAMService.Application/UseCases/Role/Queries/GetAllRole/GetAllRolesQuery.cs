using IAMService.Application.Models.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Application.UseCases.Role.Queries.GetAllRole
{
    public record GetAllRolesQuery : IRequest<List<CreateRoleDto>>
    {
    }
}
