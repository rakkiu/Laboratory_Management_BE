using IAMService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IAMService.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Background job that periodically cleans up expired refresh tokens
    /// from the database to maintain optimal performance and security.
    /// </summary>
    public class RefreshTokenCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenCleanupService"/> class.
        /// </summary>
        /// <param name="scopeFactory">Factory to create scoped services (for DbContext).</param>
        public RefreshTokenCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Executes the cleanup process in a continuous background loop.
        /// </summary>
        /// <param name="stoppingToken">Cancellation token used to gracefully stop the service.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IAMDbContext>();

                // 🧩 Truy vấn tất cả RefreshToken đã hết hạn
                var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

                var expiredTokens = await context.JwtTokens
                    .Where(rt => rt.ExpiresAt < now)
                    .ToListAsync(stoppingToken);

                // 🧹 Nếu có token hết hạn → xóa khỏi DB
                if (expiredTokens.Any())
                {
                    context.JwtTokens.RemoveRange(expiredTokens);
                    await context.SaveChangesAsync(stoppingToken);
                    Console.WriteLine($"Deleted {expiredTokens.Count} RefreshToken out of date ({DateTime.UtcNow}).");
                }

                // ⏰ Lặp lại sau mỗi 24 giờ
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
