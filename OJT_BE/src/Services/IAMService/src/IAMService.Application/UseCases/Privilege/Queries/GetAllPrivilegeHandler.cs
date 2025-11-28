using IAMService.Application.Models.Privilege;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Application.UseCases.Privilege.Queries
{
    /// <summary>
    /// Handler for getting all privileges
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAMService.Application.UseCases.Privilege.Queries.GetAllPrivilegesQuery, System.Collections.Generic.List&lt;IAMService.Application.Models.Privilege.PrivilegeDto&gt;&gt;" />
    public class GetAllPrivilegeHandler : IRequestHandler<GetAllPrivilegesQuery, List<PrivilegeDto>>
    {
        /// <summary>
        /// The privilege repository
        /// </summary>
        private readonly IPrivilegeRepository _privilegeRepository;
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllPrivilegeHandler"/> class.
        /// </summary>
        /// <param name="privilegeRepository">The privilege repository.</param>
        public GetAllPrivilegeHandler(IPrivilegeRepository privilegeRepository)
        {
            _privilegeRepository = privilegeRepository;
        }
        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<List<PrivilegeDto>> Handle(GetAllPrivilegesQuery request, CancellationToken cancellationToken)
        {
            var privileges = await _privilegeRepository.GetAllPrivilegesAsync(cancellationToken);
            return privileges.Select(p => new PrivilegeDto
            {
                PrivilegeId = p.PrivilegeId,
                PrivilegeName = p.PrivilegeName,
                Description = p.Description
            }).ToList();
        }
    }
}
