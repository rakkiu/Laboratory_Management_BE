



using IAMService.Application.Models.User;

namespace IAMService.Application.UseCases.User.Queries.GetListUser
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Collections.Generic.IEnumerable&lt;IAMService.Application.Models.User.UserDTO&gt;&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;IAMService.Application.UseCases.User.Queries.GetListUser.GetListUserQuery&gt;" />
    public record GetListUserQuery : IRequest<IEnumerable<UserDTO>>;
}
