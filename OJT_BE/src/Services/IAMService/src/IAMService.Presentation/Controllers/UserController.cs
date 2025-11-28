using IAMService.Application.UseCases.User.Commands.CreateUser;
using IAMService.Application.UseCases.User.Queries.ViewUserInformation;
using Shared.Security;
using IAMService.Application.Models.Email;


namespace IAMService.Presentation.Controllers
{
    /// <summary>
    /// Provides endpoints for managing user accounts, including viewing information,
    /// creating, locking, unlocking, and permanently deleting users.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController" /> class.
        /// </summary>
        /// <param name="mediator">The mediator used for handling requests and commands.</param>
        public UserController(IMediator mediator, IEmailService emailService)
        {
            _mediator = mediator;
            _emailService = emailService;
        }


        /// <summary>
        /// Views the detailed information of a specific user by user ID.
        /// Requires a valid token with the role <c>ADMIN</c> or <c>LAB_MANAGER</c>.
        /// </summary>
        /// <param name="userId">The user ID to retrieve information for.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// An <see cref="ApiResponse{T}" /> containing user information if found.
        /// </returns>
        [HttpGet("{userId:guid}")]
        [Authorize(Roles = "ADMIN,LAB_MANAGER")]
        [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserDTO>>> ViewUserInformation(
            [FromRoute] Guid userId,
            CancellationToken cancellationToken)
        {
            var currentUserRoleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)
                                        ?? User.FindFirst("RoleCode");

            if (currentUserRoleClaim == null || string.IsNullOrEmpty(currentUserRoleClaim.Value))
            {
                return Unauthorized(new ApiResponse<object?>
                {
                    StatusCode = 401,
                    Message = "Invalid token: Role not found",
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }

            var command = new ViewUserInformationCommand(userId, currentUserRoleClaim.Value);
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponse<UserDTO>
            {
                StatusCode = 200,
                Message = "View user information successfully",
                Data = result,
                ResponsedAt = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Retrieves a list of all users.
        /// Accessible only by users with roles <c>ADMIN</c> or <c>LAB_MANAGER</c>.
        /// </summary>
        /// <param name="cancellationToken">A token that propagates notification that the operation should be canceled.</param>
        /// <returns>
        /// An <see cref="ApiResponse{T}" /> containing the list of users.
        /// </returns>
        [HttpGet("getalluser")]
        [AuthorizeByPrivilege("View user")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDTO>>>> GetAllUsers(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetListUserQuery(), cancellationToken);

            if (result == null || !result.Any())
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "No users found",
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }

            return Ok(new ApiResponse<IEnumerable<UserDTO>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Users retrieved successfully",
                Data = result,
                ResponsedAt = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Creates a new user account.
        /// Only users with <c>ADMIN</c> or <c>LAB_MANAGER</c> roles are authorized.
        /// </summary>
        /// <param name="command">The command containing information to create the new user.</param>
        /// <param name="ct">A cancellation token for the asynchronous operation.</param>
        /// <returns>
        /// An <see cref="ApiResponse{T}" /> with the result of the user creation.
        /// </returns>
        [HttpPost("create")]
        [AuthorizeByPrivilege("Create user")]
        [ProducesResponseType(typeof(ApiResponse<CreateUserResultDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<CreateUserResultDto>>> Create([FromBody] CreateUserCommand command, CancellationToken ct)
        {

            try
            {
                var result = await _mediator.Send(command, ct);

                return StatusCode(StatusCodes.Status201Created, new ApiResponse<CreateUserResultDto>
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "User created successfully.",
                    Data = result,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = "Invalid data (e.g., RoleCode does not exist).",
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Locks (deactivates) a user account.
        /// </summary>
        /// <param name="id">The unique identifier of the user to lock.</param>
        /// <returns>
        /// An <see cref="IActionResult" /> representing the outcome of the operation.
        /// </returns>
        [HttpPost("{id:guid}/lock")]
        [AuthorizeByPrivilege("Lock/Unlock user")]

        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LockUser(Guid id)
        {
            var performingAdminId = GetPerformingAdminId();
            if (performingAdminId == Guid.Empty)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Cannot determine performing administrator.",
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }

            var command = new LockUserCommand(id, performingAdminId);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return HandleErrorResult(result.Error);
            }

            return Ok(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "User locked successfully.",
                Data = null,
                ResponsedAt = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Unlocks (reactivates) a previously locked user account.
        /// </summary>
        /// <param name="id">The unique identifier of the user to unlock.</param>
        /// <returns>
        /// An <see cref="IActionResult" /> indicating whether the operation succeeded.
        /// </returns>
        [HttpPost("{id:guid}/unlock")]
        [AuthorizeByPrivilege("Lock/Unlock user")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UnlockUser(Guid id)
        {
            var performingAdminId = GetPerformingAdminId();
            if (performingAdminId == Guid.Empty)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Cannot determine performing administrator.",
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }

            var command = new UnlockUserCommand(id, performingAdminId);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return HandleErrorResult(result.Error);
            }

            return Ok(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "User unlocked successfully.",
                Data = null,
                ResponsedAt = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Permanently deletes (purges) a user account.
        /// </summary>
        /// <param name="id">The unique identifier of the user to permanently delete.</param>
        /// <returns>
        /// An <see cref="IActionResult" /> describing the result of the deletion.
        /// </returns>
        [HttpDelete("{id:guid}/permanent")]
        [AuthorizeByPrivilege("Delete user")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteUserPermanently(Guid id)
        {
            var performingAdminId = GetPerformingAdminId();
            if (performingAdminId == Guid.Empty)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Cannot determine performing administrator.",
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }

            var command = new DeleteUserPermanentlyCommand(id, performingAdminId);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return HandleErrorResult(result.Error);
            }

            return Ok(new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "User permanently deleted.",
                Data = null,
                ResponsedAt = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Gets the authenticated administrator�s identifier from user claims.
        /// </summary>
        /// <returns>
        /// The admin�s <see cref="Guid" /> ID, or <see cref="Guid.Empty" /> if not found.
        /// </returns>
        private Guid GetPerformingAdminId()
        {
            var adminIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(adminIdStr, out var adminId);
            return adminId;
        }

        /// <summary>
        /// Converts a <see cref="DomainError" /> into an appropriate HTTP response.
        /// </summary>
        /// <param name="error">The domain error returned from the business logic layer.</param>
        /// <returns>
        /// An <see cref="IActionResult" /> representing the error.
        /// </returns>
        private IActionResult HandleErrorResult(DomainError error)
        {
            var response = new ApiResponse<object>
            {
                Message = error.Description,
                Data = null,
                ResponsedAt = DateTime.UtcNow
            };

            if (error == DomainErrors.UserError.NotFound)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                return NotFound(response);
            }

            if (error == DomainErrors.UserError.AlreadyLocked ||
                error == DomainErrors.UserError.AlreadyUnlocked )
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                return BadRequest(response);
            }

            response.StatusCode = StatusCodes.Status400BadRequest;
            return BadRequest(response);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        
        [AuthorizeByPrivilege("Modify user")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                if (request == null)
                {
                    var errorResponse = new ApiResponse<object>
                    {
                        StatusCode = 400,
                        Message = "Request body cannot be empty.",
                        Data = null,
                        ResponsedAt = DateTime.UtcNow
                    };
                    return BadRequest(errorResponse);
                }

                var command = new UpdateUserCommand(
                    UserId: id,
                    FullName: request.FullName,
                    DateOfBirth: request.DateOfBirth,
                    Age: request.Age,
                    Gender: request.Gender,
                    Address: request.Address,
                    Email: request.Email,
                    PhoneNumber: request.PhoneNumber
                );

                await _mediator.Send(command);

                var response = new ApiResponse<object>
                {
                    StatusCode = 200,
                    Message = "User information updated successfully",
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                };

                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = 404,
                    Message = $"UserId {id} not found",
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }

            
        }

        [HttpPost("send-email-with-attachment")]
        public async Task<IActionResult> SendEmailWithAttachment([FromForm] EmailAttachmentRequest request)
        {
            await _emailService.SendWithAttachmentAsync(
                request.To,
                request.Subject,
                request.Body,
                request.Attachment.FileName,
                request.Attachment.OpenReadStream()
            );

            return Ok();
        }

    }
}
