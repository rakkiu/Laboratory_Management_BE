
namespace IAMService.Application.UseCases.Users.Commands.UnlockUser
{
    /// <summary>
    /// Represents the command to unlock (activate) a user.
    /// </summary>
    /// <param name="UserId">The identifier of the user to unlock.</param>
    /// <param name="PerformingAdminId">The identifier of the admin performing the action.</param>
    public sealed record UnlockUserCommand(
        Guid UserId,
        Guid PerformingAdminId) : IRequest<Result>;
}