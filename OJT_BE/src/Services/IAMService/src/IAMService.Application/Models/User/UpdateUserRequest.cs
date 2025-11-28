namespace IAMService.Application.Models.Users
{
    public record UpdateUserRequest(
        string FullName,
        DateTime DateOfBirth,
        int Age,
        string Gender,
        string Address,
        string Email,
        string PhoneNumber
    );
}
