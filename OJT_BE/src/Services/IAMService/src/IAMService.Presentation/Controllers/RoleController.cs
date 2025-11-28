using IAMService.Application.Models.Role;
using IAMService.Application.UseCases.Role.Commands;
using IAMService.Application.UseCases.Role.Commands.CreateRole;
using IAMService.Application.UseCases.Role.Commands.DeleteRole;
using IAMService.Application.UseCases.Role.Commands.UpdateRole;
using IAMService.Application.UseCases.Role.Queries.GetAllRole;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Security;

namespace IAMService.Presentation.Controllers
{
    /// <summary>
    /// controller for role management
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleController" /> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Creates the role.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>

        [HttpPost("create")]
        [AuthorizeByPrivilege("Create role")]
        [ProducesResponseType(typeof(ApiResponse<CreateRoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CreateRoleDto>>> CreateRole([FromBody] CreateRoleCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(command, cancellationToken);
                return Ok(new ApiResponse<CreateRoleDto>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Role has been created successfully.",
                    Data = result,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (ArgumentException ae)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ae.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }
        /// <summary>
        /// Gets all roles.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet("all")]
        [AuthorizeByPrivilege("View role")]
        [ProducesResponseType(typeof(ApiResponse<List<CreateRoleDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<CreateRoleDto>>>> GetAllRoles(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllRolesQuery(), cancellationToken);
            return Ok(new ApiResponse<List<CreateRoleDto>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Roles retrieved successfully.",
                Data = result,
                ResponsedAt = DateTime.UtcNow
            });

        }
        /// <summary>
        /// Updates the role.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost("update")]
        [AuthorizeByPrivilege("Update role")]
        [ProducesResponseType(typeof(ApiResponse<UpdateRoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<UpdateRoleDto>>> UpdateRole([FromBody] UpdateRoleCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(command, cancellationToken);
                return Ok(new ApiResponse<UpdateRoleDto>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Role has been updated successfully.",
                    Data = result,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException ke)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ke.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }
        /// <summary>
        /// Deletes the role.
        /// </summary>
        /// <param name="roleCode">The role code.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpDelete("delete/{roleCode}")]
        [AuthorizeByPrivilege("Delete role")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteRole([FromRoute] string roleCode, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(new DeleteRoleCommand(roleCode), cancellationToken);
                return Ok(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Role has been deleted successfully.",
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException ke)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ke.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (ArgumentException ae)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ae.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }
    }
}
