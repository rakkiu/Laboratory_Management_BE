namespace IAMService.Application.UseCases.User.Queries.ViewUserInformation
{
    /// <summary>
    /// Command to view user information - Admin/Manager can view any user by userId
    /// </summary>
    /// <seealso cref="IRequest&lt;UserDTO&gt;" />
    public record ViewUserInformationCommand(Guid UserId, string CurrentUserRole) : IRequest<UserDTO>;
}

