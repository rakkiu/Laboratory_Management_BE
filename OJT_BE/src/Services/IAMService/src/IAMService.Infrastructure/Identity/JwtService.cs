namespace IAMService.Infrastructure.Identity
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IJwtService" />
    public class JwtService : IJwtService
    {
        /// <summary>
        /// The settings
        /// </summary>
        private readonly JwtSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtService"/> class.
        /// </summary>
        /// <param name="opts">The opts.</param>
        public JwtService(IOptions<JwtSettings> opts) => _settings = opts.Value;

        /// <summary>
        /// Generates the access token.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("FullName", user.FullName ?? string.Empty),
                new Claim("RoleCode", user.RoleCode ?? string.Empty),

                // claim động để mỗi token là duy nhất
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };
            if (user.Role?.RolePrivileges != null)
            {
                foreach (var rp in user.Role.RolePrivileges)
                {
                    if (rp.Privilege != null)
                    {
                        claims.Add(new Claim("Privilege", rp.Privilege.PrivilegeName));
                    }
                }
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpiresMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generates the refresh token.
        /// </summary>
        /// <returns></returns>
        public string GenerateRefreshToken()
        {
            var random = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);
            return Convert.ToBase64String(random);
        }

        /// <summary>
        /// Validates the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="validateLifetime">if set to <c>true</c> [validate lifetime].</param>
        /// <returns></returns>
        public ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_settings.SecretKey);
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Audience,
                ValidateLifetime = validateLifetime,
                ClockSkew = TimeSpan.FromSeconds(30)
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, parameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the access token expiration in minutes from settings.
        /// </summary>
        /// <returns>Access token expiration in minutes</returns>
        public int GetAccessTokenExpirationMinutes()
        {
            return _settings.AccessTokenExpiresMinutes;
        }

        /// <summary>
        /// Gets the refresh token expiration in days from settings.
        /// </summary>
        /// <returns>Refresh token expiration in days</returns>
        public int GetRefreshTokenExpirationDays()
        {
            return _settings.RefreshTokenExpiresDays;
        }
    }
}
