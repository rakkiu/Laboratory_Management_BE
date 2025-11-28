using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.UseCases.CommentsUC.Add;
using PatientService.Application.UseCases.CommentsUC.Delete;
using PatientService.Application.UseCases.CommentsUC.Modify;
using PatientService.Application.Dtos.CommentsDto;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PatientService.Presentation.Controllers
{
    /// <summary>
    /// API controller for managing comments on Test Orders.
    /// </summary>
    [ApiController]
    [Route("api/test-orders/{testOrderId}/comments")]
    public class CommentsController : ControllerBase
    {
        /// <summary>
        /// The MediatR ISender interface for dispatching commands and queries.
        /// </summary>
        private readonly ISender _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestOrderCommentsController"/> class.
        /// </summary>
        /// <param name="mediator">The MediatR sender, injected by DI.</param>
        public CommentsController(ISender mediator)
        {
            this._mediator = mediator;
        }

        /// <summary>
        /// Retrieves the 'FullName' claim of the authenticated user.
        /// </summary>
        /// <returns>The user's full name, or "unknown.user" if the claim is not found.</returns>
        private string GetCurrentUserName()
        {
            // Attempts to find the "FullName" claim from the authenticated user's token.
            var userName = User.FindFirstValue("FullName");
            return userName ?? "unknown.user";
        }

        /// <summary>
        /// Adds a new comment to a specific test order.
        /// </summary>
        /// <remarks>
        /// Endpoint: POST api/test-orders/{testOrderId}/comments
        /// </remarks>
        /// <param name="testOrderId">The unique identifier of the test order.</param>
        /// <param name="requestDto">The data transfer object containing the comment content.</param>
        /// <returns>An <see cref="IActionResult"/> containing the newly created <see cref="CommentResponseDto"/>.</returns>
        [HttpPost]
        public async Task<IActionResult> AddComment(Guid testOrderId, [FromBody] AddCommentRequestDto requestDto)
        {
            try
            {
                var command = new AddCommentCommand
                {
                    TestOrderId = testOrderId,
                    Content = requestDto.Content,
                    CurrentUserName = GetCurrentUserName(),
                    UserName = requestDto.CreatedBy
                };

                var commentDto = await _mediator.Send(command);

                // Return a 201 Created status with the new comment data
                return Ok(new ApiResponse<CommentResponseDto>
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Comment created successfully.",
                    Data = commentDto,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex) // 400 Bad Request
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException ex) // 404 Not Found
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex) // 500 Internal Server Error
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An unexpected error occurred: " + ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Modifies the content of an existing comment.
        /// </summary>
        /// <remarks>
        /// Endpoint: PUT api/test-orders/{testOrderId}/comments/{commentId}
        /// </remarks>
        /// <param name="testOrderId">The unique identifier of the test order (from route).</param>
        /// <param name="commentId">The unique identifier of the comment to modify.</param>
        /// <param name="requestDto">The data transfer object with the new comment content.</param>
        /// <returns>An <see cref="IActionResult"/> indicating success (200 OK) or an error.</returns>
        [HttpPut("{commentId}")]
        public async Task<IActionResult> ModifyComment(Guid testOrderId, Guid commentId, [FromBody] ModifyCommentRequestDto requestDto)
        {
            try
            {
                var command = new ModifyCommentCommand
                {
                    CommentId = commentId,
                    NewContent = requestDto.NewContent,
                    CurrentUserName = GetCurrentUserName()

                };

                await _mediator.Send(command);

                return Ok(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Comment modified successfully.",
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex) // 400 Bad Request
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException ex) // 404 Not Found
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex) // 500 Internal Server Error
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An unexpected error occurred: " + ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Deletes a specific comment.
        /// </summary>
        /// <remarks>
        /// Endpoint: DELETE api/test-orders/{testOrderId}/comments/{commentId}
        /// </remarks>
        /// <param name="testOrderId">The unique identifier of the test order (from route).</param>
        /// <param name="commentId">The unique identifier of the comment to delete.</param>
        /// <returns>An <see cref="IActionResult"/> indicating success (200 OK) or an error.</returns>
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid testOrderId, Guid commentId)
        {
            try
            {
                var command = new DeleteCommentCommand
                {
                    CommentId = commentId,
                    CurrentUserName = GetCurrentUserName(),
                };

                await _mediator.Send(command);

                return Ok(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Comment deleted successfully.",
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException ex) // 404 Not Found
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex) // 400 Bad Request
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex) // 500 Internal Server Error
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An unexpected error occurred: " + ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }
    } 
}