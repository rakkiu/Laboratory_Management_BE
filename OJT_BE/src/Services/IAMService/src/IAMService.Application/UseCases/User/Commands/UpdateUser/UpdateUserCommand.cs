

namespace IAMService.Application.UseCases.Users.Commands;
/// <summary>
/// Represents a command to update an existing user's information in the system.
/// </summary>
/// <param name="UserId">The unique identifier (GUID) of the user to be updated.</param>
/// <param name="FullName">The full name of the user.</param>
/// <param name="DateOfBirth">The date of birth of the user.</param>
/// <param name="Age">The age of the user, calculated from the date of birth.</param>
/// <param name="Gender">The gender of the user.</param>
/// <param name="Address">The residential address of the user.</param>
/// <param name="Email">The email address of the user.</param>
/// <param name="PhoneNumber">The contact phone number of the user.</param>
public record UpdateUserCommand(
    Guid UserId,
    string FullName,
    DateTime DateOfBirth,
    int Age,
    string Gender,
    string Address,
    string Email,
    string PhoneNumber
) : IRequest<Unit>;