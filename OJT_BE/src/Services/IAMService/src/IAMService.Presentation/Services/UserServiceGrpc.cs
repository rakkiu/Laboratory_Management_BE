using Grpc.Core;
using Shared.GrpcContracts;

namespace IAMService.Presentation.Services
{
    public class UserServiceGrpc : UserService.UserServiceBase
    {
        private readonly IUserRepository _userRepository;

        public UserServiceGrpc(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public override async Task<GetUserByIdReply> GetUserById(GetUserByIdRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out var guid))
            {
                return new GetUserByIdReply { Found = false, UserId = request.UserId, FullName = string.Empty };
            }

            var user = await _userRepository.GetByIdAsync(guid); 
            if (user == null)
            {
                return new GetUserByIdReply { Found = false, UserId = request.UserId, FullName = string.Empty };
            }

            return new GetUserByIdReply
            {
                Found = true,
                UserId = user.UserId.ToString(),
                FullName = user.FullName ?? $"{user.RoleCode} user"
            };
        }
    }
}
