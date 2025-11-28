using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Models.FlaggingSetDto;
using PatientService.Application.UseCases.FlaggingSet.Command;
using PatientService.Application.UseCases.FlaggingSet.Command.UpdateFlagging;
using PatientService.Application.UseCases.FlaggingSet.Command.ViewFlag;
using PatientService.Domain.Interfaces;

namespace PatientService.Presentation.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/flagging-configs")]
    [ApiController]

    public class FlaggingSetController : ControllerBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;
        /// <summary>
        /// The flagging set repository
        /// </summary>
        private readonly IFlaggingSetRepository _flaggingSetRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlaggingSetController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <param name="flaggingSetRepository">The flagging set repository.</param>
        public FlaggingSetController(IMediator mediator, IFlaggingSetRepository flaggingSetRepository)
        {
            _mediator = mediator;
            _flaggingSetRepository = flaggingSetRepository;
        }

        /// <summary>
        /// Retrieves all flagging set configurations.
        /// </summary>
        /// <returns>A list of all flagging set configurations.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<FlaggingSetConfigDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<FlaggingSetConfigDto>>>> GetAllFlaggingConfigs()
        {
            try
            {
                var query = new ViewAllFlagCommand();
                var result = await _mediator.Send(query);

                return Ok(new ApiResponse<IEnumerable<FlaggingSetConfigDto>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Flagging configurations retrieved successfully.",
                    Data = result,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                // Log the exception (ex)
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An internal server error occurred while retrieving flagging configurations.",
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Creates a new flagging set configuration.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<FlaggingSetConfigDto>>> CreateFlaggingConfig([FromBody] CreateFlaggingSetCommand command)
        {
            try
            {
                // Perform conflict check directly in the controller
                var existing = await _flaggingSetRepository.GetByTestNameAsync(command.TestName);
                if (existing != null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status409Conflict,
                        Message = "A configuration for this test already exists.",
                        ResponsedAt = DateTime.UtcNow
                    });
                }

                var result = await _mediator.Send(command);
                return Ok(new ApiResponse<FlaggingSetConfigDto>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Flagging configuration created successfully.",
                    Data = result,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid request. Please check input fields.",
                    Data = ex.Errors,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An internal server error occurred.",
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Updates a flagging set configuration by its ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ApiResponse<FlaggingSetConfigDto>>> UpdateFlaggingConfig(int id, [FromBody] UpdateFlaggingSetCommand command)
        {
            try
            {
                // Gán ID từ route vào command
                command.ConfigId = id;

                var result = await _mediator.Send(command);

                // Check for null result from handler to indicate "Not Found"
                if (result == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = $"Flagging configuration with ID {id} not found.",
                        ResponsedAt = DateTime.UtcNow
                    });
                }

                return Ok(new ApiResponse<FlaggingSetConfigDto>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Flagging configuration updated successfully.",
                    Data = result,
                    ResponsedAt = DateTime.UtcNow
                });


            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid request. Please check input fields.",
                    Data = ex.Errors,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An internal server error occurred.",
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }
    }
}