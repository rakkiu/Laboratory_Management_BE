namespace IAMService.Application.UseCases.User.Commands.CreateUser
{
    /// <summary>
    /// Lệnh tạo người dùng (chỉ Admin/Lab Manager sử dụng)
    /// </summary>
    public class CreateUserCommand : IRequest<CreateUserResultDto>
    {
        public string RoleCode { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string PhoneNumber { get; init; } = null!;
        public string FullName { get; init; } = null!;
        public string IdentifyNumber { get; init; } = null!;
        public string Gender { get; init; } = null!;
        public int Age { get; init; }
        public string Address { get; init; } = null!;
        // Yêu cầu format MM/dd/yyyy theo đặc tả
        public string DateOfBirth { get; init; } = null!;
    }
}