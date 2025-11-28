using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.UseCases.TestOrderUC.TestOrderResult.Command.CreateTestOrderResult;
using PatientService.Application.UseCases.TestOrderUC.TRSyncUp.Command.CreateTRSyncUp;

namespace PatientService.Presentation.Controllers
{
    [Route("api/CreateResult")]
    [ApiController]
    public class TestOrderResultController : ControllerBase
    {
        private readonly IMediator mediator;
        
        public TestOrderResultController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> CreateTestResult(CreateTestOrderResultCommand request)
        {
            try
            {
                request.EnteredBy ??= User?.Identity?.Name ?? "System";

                await mediator.Send(request);
                return Ok(new ApiResponse<object>
                {
                    ResponsedAt = DateTime.Now,
                    Data = true,
                    Message = "Test result created successfully",
                    StatusCode = StatusCodes.Status201Created
                });
            }
            catch (ArgumentException ae)
            {
                return BadRequest(new ApiResponse<object>
                {
                    ResponsedAt = DateTime.Now,
                    Data = null,
                    Message = ae.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    ResponsedAt = DateTime.Now,
                    Data = null,
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPost("sync")]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTestResultSyncUp([FromBody] CreateTRSyncUpCommand command)
        {
            try
            {
                var resultId = await mediator.Send(command);

                return CreatedAtAction(
                    nameof(CreateTestResult),
                    new ApiResponse<Guid>
                    {
                        StatusCode = StatusCodes.Status201Created,
                        Message = "Test result created successfully.",
                        Data = resultId,
                        ResponsedAt = DateTime.UtcNow
                    });
            }
            catch (KeyNotFoundException knfe)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = knfe.Message,
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
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An unexpected error occurred.",
                    Data = ex.Message,
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }

    }
}
