using IAMService.Application.Models.Privilege;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Application.UseCases.Privilege.Queries
{
    /// <summary>
    /// query for getting all privileges
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Collections.Generic.List&lt;IAMService.Application.Models.Privilege.PrivilegeDto&gt;&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;IAMService.Application.UseCases.Privilege.Queries.GetAllPrivilegesQuery&gt;" />
    public record GetAllPrivilegesQuery : IRequest<List<PrivilegeDto>>
    {
    }
}
