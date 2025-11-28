using System;
using MediatR;

namespace IAMService.Application.UseCases.Auth.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest<Unit>
    {
    }
}