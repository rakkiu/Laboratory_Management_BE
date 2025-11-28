namespace IAMService.Application.Features.Users.Commands;

/// <summary>
/// Handles the <see cref="UpdateUserCommand"/> to update user information.
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository instance.</param>
    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Processes the user update request.
    /// </summary>
    /// <param name="request">The update command containing user details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns <see cref="Unit.Value"/> when the update is completed.</returns>
    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userToUpdate = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (userToUpdate is null)
        {
            throw new KeyNotFoundException($"User with ID {request.UserId} not found.");
        }

        userToUpdate.FullName = request.FullName;
        userToUpdate.DateOfBirth = request.DateOfBirth.Date;
        userToUpdate.Age = request.Age;
        userToUpdate.Gender = request.Gender;
        userToUpdate.Address = request.Address;
        userToUpdate.Email = request.Email;
        userToUpdate.PhoneNumber = request.PhoneNumber;

        _userRepository.Update(userToUpdate);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
