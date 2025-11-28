namespace IAMService.Application.UseCases.Users.Commands.DeleteUserPermanently
{
    /// <summary>
    /// Represents the command to permanently delete (purge) a user.
    /// </summary>
    /// <param name="UserId">The identifier of the user to permanently delete.</param>
    /// <param name="PerformingAdminId">The identifier of the admin performing the action.</param>
    public sealed record DeleteUserPermanentlyCommand(
        Guid UserId,
        Guid PerformingAdminId) : IRequest<Result>;
}