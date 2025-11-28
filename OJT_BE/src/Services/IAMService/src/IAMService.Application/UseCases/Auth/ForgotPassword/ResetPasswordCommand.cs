namespace IAMService.Application.UseCases.Auth.ForgotPassword
{
    public record ResetPasswordCommand(string Token, string NewPassword) : IRequest<Unit>;
}
