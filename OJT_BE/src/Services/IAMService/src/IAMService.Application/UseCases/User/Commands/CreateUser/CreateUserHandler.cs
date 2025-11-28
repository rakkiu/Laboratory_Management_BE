using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace IAMService.Application.UseCases.User.Commands.CreateUser
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAMService.Application.UseCases.User.Commands.CreateUser.CreateUserCommand, IAMService.Application.Models.User.CreateUserResultDto&gt;" />
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResultDto>
    {
        /// <summary>
        /// The repo
        /// </summary>
        private readonly IUserRepository _repo;
        /// <summary>
        /// The email service
        /// </summary>
        private readonly IEmailService _emailService;
        /// <summary>
        /// The role repo
        /// </summary>
        private readonly IRoleRepository _roleRepo;

        /// <summary>
        /// The acceptable dob formats
        /// </summary>
        private static readonly string[] AcceptableDobFormats = new[]
        {
            "MM/dd/yyyy"
        };
        /// <summary>
        /// The email regex
        /// </summary>
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateUserHandler"/> class.
        /// </summary>
        /// <param name="repo">The repo.</param>
        /// <param name="emailService">The email service.</param>
        /// <param name="roleRepo">The role repo.</param>
        public CreateUserHandler(IUserRepository repo, IEmailService emailService, IRoleRepository roleRepo)
        {
            _repo = repo;
            _emailService = emailService;
            _roleRepo = roleRepo;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// Creating an ADMIN account via this use case is not allowed.
        /// or
        /// Invalid email format.
        /// or
        /// Email already exists.
        /// or
        /// Invalid date of birth. Accepted: MM/dd/yyyy.
        /// or
        /// Role does not exist.
        /// or
        /// Identify Number must be between 5 and 20 characters.
        /// or
        /// Phone Number must be between 8 and 20 characters.
        /// </exception>
        public async Task<CreateUserResultDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var roleCode = request.RoleCode?.Trim();
            var email = request.Email?.Trim();
            var phone = request.PhoneNumber?.Trim();
            var fullName = request.FullName?.Trim();
            var idNo = request.IdentifyNumber?.Trim();
            var genderRaw = request.Gender?.Trim();
            var address = request.Address?.Trim();
            var dobText = request.DateOfBirth?.Trim();

            if (string.Equals(roleCode, "ADMIN", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Creating an ADMIN account via this use case is not allowed.");
            if (!EmailRegex.IsMatch(email))
                throw new ArgumentException("Invalid email format.");
            var existed = await _repo.GetByEmailAsync(email!, cancellationToken);
            if (existed != null)
                throw new ArgumentException("Email already exists.");

            if (!DateTime.TryParseExact(dobText, AcceptableDobFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob))
                throw new ArgumentException("Invalid date of birth. Accepted: MM/dd/yyyy.");
            if(!await _roleRepo.RoleExistsAsync(roleCode))
                throw new ArgumentException("Role does not exist.");
            if(idNo.Length < 6 || idNo.Length > 20)
                throw new ArgumentException("Identify Number must be between 5 and 20 characters.");
            if(phone.Length < 8 || phone.Length > 20)
                throw new ArgumentException("Phone Number must be between 8 and 20 characters.");
            string genderNorm = genderRaw?.ToLowerInvariant() switch
            {
                "male" or "m" => "Male",
                "female" or "f" => "Female",
                _ => throw new ArgumentException("Gender must be one of: Male, Female, M, F.")
            };

            var plainPassword = GenerateRandomPassword(12);

            var user = new Domain.Entities.User
            {
                UserId = Guid.NewGuid(),
                RoleCode = roleCode!,
                Email = email!,
                PhoneNumber = phone!,
                FullName = fullName!,
                IdentifyNumber = idNo!,
                Gender = genderNorm,
                Age = request.Age,
                Address = address!,
                DateOfBirth = dob,
                Password = BCrypt.Net.BCrypt.HashPassword(plainPassword),
                IsActive = true
            };

            await _repo.AddAsync(user, cancellationToken);
            await _repo.SaveChangesAsync(cancellationToken);
            user = await _repo.GetByIdAsync(user.UserId, cancellationToken);
            // send email notification
            try {
                var subject = "Your Account Has Been Created";
                var body = $@"Hello {user.FullName},
Your account has been successfully created.
Login email: {user.Email}
Temporary password: {plainPassword}
Please change your password after logging in for security purposes.";
                await _emailService.SendAsync(user.Email, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email to {user.Email}: {ex.Message}");
            }


            return new CreateUserResultDto
            {
                UserId = user.UserId,
                RoleCode = user.RoleCode,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                IdentifyNumber = user.IdentifyNumber,
                Gender = user.Gender,
                Age = user.Age,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth
            };
        }
        /// <summary>
        /// Generates the random password.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        private static string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            var data = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(data);
            var sb = new StringBuilder(length);
            foreach (var b in data)
                sb.Append(chars[b % chars.Length]);
            return sb.ToString();
        }
    }
}