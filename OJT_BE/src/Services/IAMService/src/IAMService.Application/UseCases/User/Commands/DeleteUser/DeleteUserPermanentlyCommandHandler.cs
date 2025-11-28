
namespace IAMService.Application.UseCases.Users.Commands.DeleteUserPermanently
{
    /// <summary>
    /// Handles the <see cref="DeleteUserPermanentlyCommand"/> request, 
    /// which permanently deletes (hard deletes) a user that was previously soft-deleted.
    /// </summary>
    public sealed class DeleteUserPermanentlyCommandHandler
        : IRequestHandler<DeleteUserPermanentlyCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenRepository _refreshTokenRepository;
        private readonly ILogger<DeleteUserPermanentlyCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteUserPermanentlyCommandHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository instance used to access user data.</param>
        /// <param name="refreshTokenRepository">The refresh token repository instance used to manage token data.</param>
        /// <param name="logger">The logger instance for recording audit and diagnostic information.</param>
        public DeleteUserPermanentlyCommandHandler(
            IUserRepository userRepository,
            IJwtTokenRepository refreshTokenRepository,
            ILogger<DeleteUserPermanentlyCommandHandler> logger)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the process of permanently deleting a user from the system.
        /// </summary>
        /// <param name="request">The <see cref="DeleteUserPermanentlyCommand"/> containing user ID and admin ID information.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>
        /// A <see cref="Result"/> indicating whether the operation succeeded or failed.  
        /// Returns <c>Result.Failure</c> if the user is not found or not soft-deleted, otherwise <c>Result.Success</c>.
        /// </returns>
        public async Task<Result> Handle(DeleteUserPermanentlyCommand request, CancellationToken cancellationToken)
        {
            // 1️ Retrieve the user by ID
            var userToDelete = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (userToDelete == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for permanent deletion.", request.UserId);
                return Result.Failure(DomainErrors.UserError.NotFound);
            }

            

            // 2 Remove any associated refresh tokens
            if (userToDelete.JwtTokens != null && userToDelete.JwtTokens.Any())
            {
                foreach (var token in userToDelete.JwtTokens)
                {
                    await _refreshTokenRepository.RemoveTokenAsync(token, cancellationToken);
                }
            }

            // 3 Remove the user permanently from the repository
            _userRepository.Remove(userToDelete);
            await _userRepository.SaveChangesAsync(cancellationToken);

            // 4 Log audit information for traceability
            _logger.LogInformation(
                "AUDIT: User account PERMANENTLY DELETED. UserId: {UserId}, DeletedByAdminId: {AdminId}",
                request.UserId, request.PerformingAdminId);

            return Result.Success();
        }
    }
}
