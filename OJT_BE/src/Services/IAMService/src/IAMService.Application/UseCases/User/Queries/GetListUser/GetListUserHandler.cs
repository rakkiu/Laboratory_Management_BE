

using AutoMapper;
using IAMService.Application.Models.User;

namespace IAMService.Application.UseCases.User.Queries.GetListUser
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAMService.Application.UseCases.User.Queries.GetListUser.GetListUserQuery, System.Collections.Generic.IEnumerable&lt;IAMService.Application.Models.User.UserDTO&gt;&gt;" />
    public class GetListUserHandler : IRequestHandler<GetListUserQuery, IEnumerable<UserDTO>>
    {
        /// <summary>
        /// The user repository
        /// </summary>
        private readonly IUserRepository _userRepository;
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// Initializes a new instance of the <see cref="GetListUserHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="mapper">The mapper.</param>
        public GetListUserHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<IEnumerable<UserDTO>> Handle(GetListUserQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetListUser();
            var userDTOs = _mapper.Map<IEnumerable<UserDTO>>(users);
            return userDTOs;
        }
    }
}
