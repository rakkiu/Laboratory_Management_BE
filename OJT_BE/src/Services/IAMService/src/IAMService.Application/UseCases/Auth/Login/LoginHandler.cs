namespace IAMService.Application.UseCases.Auth.Login
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IRequestHandler&lt;LoginCommand, LoginResultDto&gt;" />
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResultDto>
    {
        /// <summary>
        /// The repo
        /// </summary>
        private readonly IUserRepository _repo;
        /// <summary>
        /// The JWT
        /// </summary>
        private readonly IJwtService _jwt;
        /// <summary>
        /// The JWT Token Repository
        /// </summary>
        private readonly IJwtTokenRepository _jwtTokenRepo;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginHandler"/> class.
        /// </summary>
        /// <param name="repo">The repo.</param>
        /// <param name="jwt">The JWT.</param>
        /// <param name="jwtTokenRepo">The JWT token repository.</param>
        public LoginHandler(IUserRepository repo, IJwtService jwt, IJwtTokenRepository jwtTokenRepo)
        {
            _repo = repo;
            _jwt = jwt;
            _jwtTokenRepo = jwtTokenRepo;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="UnauthorizedAccessException">Invalid email or password.</exception>
        public async Task<LoginResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Validate user credentials
            var user = await _repo.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                throw new UnauthorizedAccessException("Invalid email or password.");
            
            if(user.IsActive == false)
                throw new UnauthorizedAccessException("User account is deactivated.");
            
            // Generate tokens
            var accessToken = _jwt.GenerateAccessToken(user);
            var refreshToken = _jwt.GenerateRefreshToken();

            // Get expiration times from settings
            var accessTokenExpirationMinutes = _jwt.GetAccessTokenExpirationMinutes();
            var refreshTokenExpirationDays = _jwt.GetRefreshTokenExpirationDays();

            // Save AccessToken to database
            var accessTokenEntity = new JwtToken
            {
                Token = accessToken,
                TokenType = "AccessToken",
                ExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                IsRevoked = false,
                UserId = user.UserId
            };

            // Save RefreshToken to database
            var refreshTokenEntity = new JwtToken
            {
                Token = refreshToken,
                TokenType = "RefreshToken",
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays),
                IsRevoked = false,
                UserId = user.UserId
            };

            // Save both tokens to database
            await _jwtTokenRepo.SaveTokenAsync(accessTokenEntity, cancellationToken);
            await _jwtTokenRepo.SaveTokenAsync(refreshTokenEntity, cancellationToken);
            _repo.Update(user); await _repo.SaveChangesAsync();
            return new LoginResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }
    }
}
