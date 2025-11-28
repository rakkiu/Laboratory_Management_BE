using IAMService.Application.Models.Role;

namespace IAMService.Application.UseCases.User.Queries.ViewUserInformation
{
    /// <summary>
    /// Handler for ViewUserInformationCommand
    /// </summary>
    /// <seealso cref="IRequestHandler&lt;ViewUserInformationCommand, UserDTO&gt;" />
    public class ViewUserInformationHandler : IRequestHandler<ViewUserInformationCommand, UserDTO>
    {
        /// <summary>
        /// The user repository
        /// </summary>
        private readonly IUserRepository _userRepo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewUserInformationHandler"/> class.
        /// </summary>
        /// <param name="userRepo">The user repo.</param>
        public ViewUserInformationHandler(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.KeyNotFoundException">User not found or has been deleted.</exception>
        public async Task<UserDTO> Handle(ViewUserInformationCommand request, CancellationToken cancellationToken)
        {
            // RoleCode đã được extract từ JWT token ở Controller
            // Validator đã check role phải là ADMIN hoặc LAB_MANAGER
            // Admin/Manager có thể xem thông tin bất kỳ user nào bằng UserId

            // Get user information từ database
            var user = await _userRepo.GetByIdWithRoleAsync(request.UserId, cancellationToken);
            if (user == null || !user.IsActive)
            {
                throw new KeyNotFoundException("User not found or has been deleted.");
            }

            // Map to DTO
            return new UserDTO
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IdentifyNumber = user.IdentifyNumber,
                Gender = user.Gender,
                Age = user.Age,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth.ToString("dd/MM/yyyy"),
                IsActive = user.IsActive,

                Role = new RoleDTO
                {
                    RoleName = user.Role?.RoleName ?? string.Empty,
                    RoleCode = user.Role?.RoleCode ?? string.Empty,
                    RoleDescription = user.Role?.RoleDescription ?? string.Empty
                }
            };
        }
    }
}

