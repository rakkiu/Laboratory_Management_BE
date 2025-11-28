namespace IAMService.Application.UseCases.Auth.Login
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IRequest&lt;LoginResultDto&gt;" />
    /// <seealso cref="IBaseRequest" />
    /// <seealso cref="IEquatable&lt;LoginCommand&gt;" />
    public record LoginCommand(string Email, string Password) : IRequest<LoginResultDto>;
}
