using IAMService.Application.Models.Privilege;
using IAMService.Application.UseCases.Privilege.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Security;

namespace IAMService.Presentation.Controllers
{
    /// <summary>
    /// controller for privilege-related operations
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/privilege")]
    [ApiController]
    public class PrivilegeController : ControllerBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;
        /// <summary>
        /// Initializes a new instance of the <see cref="PrivilegeController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public PrivilegeController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Gets all privileges.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizeByPrivilege("View role")]
        [ProducesResponseType(typeof(ApiResponse<List<PrivilegeDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<PrivilegeDto>>>> GetAllPrivileges(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllPrivilegesQuery(), cancellationToken);
            return Ok(new ApiResponse<List<PrivilegeDto>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Privileges retrieved successfully.",
                Data = result,
                ResponsedAt = DateTime.UtcNow
            });
        }
    }
}
