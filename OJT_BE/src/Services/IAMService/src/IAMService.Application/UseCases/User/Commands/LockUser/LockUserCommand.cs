
namespace IAMService.Application.UseCases.Users.Commands.LockUser
{
    /// <summary>
    /// Represents the command to lock (deactivate) a user.
    /// </summary>
    /// <param name="UserId">The identifier of the user to lock.</param>
    /// <param name="PerformingAdminId">The identifier of the admin performing the action.</param>
    public sealed record LockUserCommand(
        Guid UserId,
        Guid PerformingAdminId) : IRequest<Result>;
}