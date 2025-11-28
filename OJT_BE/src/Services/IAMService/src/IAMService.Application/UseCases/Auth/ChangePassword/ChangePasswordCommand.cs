

namespace IAMService.Application.UseCases.Auth.ChangePassword
{
    public record ChangePasswordCommand(
        Guid UserId,
        string CurrentPassword,
        string NewPassword
    ) : IRequest<Unit>;
}