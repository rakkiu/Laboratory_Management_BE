namespace IAMService.Application.Models.User
{
    /// <summary>
    /// Kết quả trả về sau khi tạo người dùng
    /// </summary>
    public class CreateUserResultDto
    {
        public Guid UserId { get; set; }
        public string RoleCode { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string IdentifyNumber { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public int Age { get; set; }
        public string Address { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
    }
}