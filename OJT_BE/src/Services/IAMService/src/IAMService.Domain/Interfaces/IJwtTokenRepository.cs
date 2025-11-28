
namespace IAMService.Domain.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IJwtTokenRepository
    {
        /// <summary>
        /// Gets the by token asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        Task<JwtToken?> GetByTokenAsync(string token);
        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns></returns>
        Task DeleteAsync(string refreshToken);
        /// <summary>
        /// Saves the refresh token asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        Task SaveTokenAsync(JwtToken token, CancellationToken ct = default);
        /// <summary>
        /// Gets the refresh token asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        Task<JwtToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default);
        /// <summary>
        /// Removes the refresh token asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        Task RemoveTokenAsync(JwtToken token, CancellationToken ct = default);
        Task<JwtToken?> GetAccessTokenByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task UpdateAsync(JwtToken refreshToken, CancellationToken cancellationToken);
        Task SaveChangeAsync(CancellationToken ct = default);

        /// <summary>
        /// Update token without saving (for batch operations).
        /// </summary>
        void Update(JwtToken token);
        /// <summary>Saves the reset token asynchronous.</summary>
        /// <param name="token">The token.</param>
        /// <param name="ct">The ct.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        Task SaveResetTokenAsync(JwtToken token, CancellationToken ct = default);
    }
}
