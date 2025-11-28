namespace IAMService.Application.UseCases.Users.Commands.LockUser
{
    /// <summary>
    /// Handles the <see cref="LockUserCommand"/>.
    /// </summary>
    public sealed class LockUserCommandHandler : IRequestHandler<LockUserCommand, Result>
    {
        /// <summary>
        /// The user repository.
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<LockUserCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LockUserCommandHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="logger">The logger.</param>
        public LockUserCommandHandler(
            IUserRepository userRepository,
            ILogger<LockUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the specified <see cref="LockUserCommand"/>.
        /// </summary>
        /// <param name="request">The command request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Result"/> indicating the outcome of the operation.</returns>
        public async Task<Result> Handle(LockUserCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the user to be locked from the repository.
            var userToLock = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            // Check if the user exists.
            if (userToLock == null)
            {
                return Result.Failure(DomainErrors.UserError.NotFound);
            }

            // Business rule check: Ensure the user is not already locked.
            if (userToLock.IsActive == false)
            {
                // Return failure if the user is already inactive (locked).
                return Result.Failure(DomainErrors.UserError.AlreadyLocked);
            }

            // Lock the user account.
            userToLock.IsActive = false;

            // Revoke all active refresh tokens associated with the user.
            foreach (var token in userToLock.JwtTokens.Where(t => !t.IsRevoked))
            {
                token.IsRevoked = true;
            }

            // Update the user in the repository and save changes.
            _userRepository.Update(userToLock);
            await _userRepository.SaveChangesAsync(cancellationToken);

            // Log the audit event for locking the user.
            _logger.LogInformation(
                "AUDIT: User account LOCKED. UserId: {UserId}, LockedByAdminId: {AdminId}",
                request.UserId, request.PerformingAdminId);

            // Return a success result.
            return Result.Success();
        }
    }
}