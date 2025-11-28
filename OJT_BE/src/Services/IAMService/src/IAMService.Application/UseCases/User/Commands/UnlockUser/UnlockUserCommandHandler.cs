namespace IAMService.Application.UseCases.Users.Commands.UnlockUser
{
    /// <summary>
    /// Handles the <see cref="UnlockUserCommand"/>.
    /// </summary>
    public sealed class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, Result>
    {
        /// <summary>
        /// The user repository.
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UnlockUserCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnlockUserCommandHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="logger">The logger.</param>
        public UnlockUserCommandHandler(
            IUserRepository userRepository,
            ILogger<UnlockUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the specified <see cref="UnlockUserCommand"/>.
        /// </summary>
        /// <param name="request">The command request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Result"/> indicating the outcome of the operation.</returns>
        public async Task<Result> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
        {
            // Note: No need to include RefreshTokens as they are not modified during unlock.
            // Retrieve the user to be unlocked from the repository.
            var userToUnlock = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            // Check if the user exists.
            if (userToUnlock == null)
            {
                return Result.Failure(DomainErrors.UserError.NotFound);
            }

            // Business rule check: Ensure the user is not already unlocked.
            if (userToUnlock.IsActive == true)
            {
                // Return failure if the user is already active (unlocked).
                return Result.Failure(DomainErrors.UserError.AlreadyUnlocked);
            }

            // Logic: Unlock the user account.
            userToUnlock.IsActive = true;

            // Update the user in the repository and save changes.
            _userRepository.Update(userToUnlock);
            await _userRepository.SaveChangesAsync(cancellationToken);

            // Log the audit event for unlocking the user.
            _logger.LogInformation(
                "AUDIT: User account UNLOCKED. UserId: {UserId}, UnlockedByAdminId: {AdminId}",
                request.UserId, request.PerformingAdminId);

            // Return a success result.
            return Result.Success();
        }
    }
}