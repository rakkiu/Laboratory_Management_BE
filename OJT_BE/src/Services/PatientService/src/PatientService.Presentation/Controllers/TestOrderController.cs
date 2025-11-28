using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.UseCases.TestOrderUC.Commands.ModifyPatientTestOrder;
using PatientService.Application.UseCases.TestOrderUC.TestOrders.Commands.CreateTestOrder;
using PatientService.Application.UseCases.TestOrderUC.TestOrders.Commands.CreateTestOrderExistPatient;
using PatientService.Application.UseCases.TestOrderUC.TestOrders.Commands.DeleteTestOrder;
using PatientService.Application.UseCases.TestOrderUC.ViewDetailPatientTestOrder.Queries;
using PatientService.Application.UseCases.TestOrderUC.ViewPatientTestOrder;
using PatientService.Application.UseCases.TestOrderUC.ViewPatientTestOrder.Queries;
using System.Security.Claims;

namespace PatientService.Presentation.Controllers
{
    [Route("api/testorder")]
    [ApiController]
    public class TestOrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TestOrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Helper: l?y UserId t? JWT
        private string? GetUserId()
        {
            return User.FindFirst("sub")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? null;
        }

        // ======================
        //        CREATE NEW PATIENT
        // ======================
        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<object>>> CreateTestOrder(
            [FromBody] CreateTestOrderCommand createTestOrderCommand)
        {
            try
            {
                //createTestOrderCommand.CreatedBy = GetUserId();

                await _mediator.Send(createTestOrderCommand);

                return Ok(new ApiResponse<object>
                {
                    StatusCode = 201,
                    Message = "Test order created successfully.",
                    Data = true,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (ArgumentException ae)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = ae.Message,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }

        // ======================
        //    CREATE EXISTING PATIENT
        // ======================
        [HttpPost("create/{patientId}")]
        public async Task<ActionResult<ApiResponse<object>>> CreateTestOrderExistPatient(
            Guid patientId,
            [FromBody] CreateTestOrderExistPatientCommand command)
        {
            try
            {
                command.PatientId = patientId;
                command.CreatedBy = GetUserId();

                await _mediator.Send(command);

                return Ok(new ApiResponse<object>
                {
                    StatusCode = 201,
                    Message = "Test order for existing patient created successfully.",
                    Data = true,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException knfe)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = 404,
                    Message = knfe.Message,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (ArgumentException ae)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = ae.Message,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    StatusCode = 500,
                    Message = "An unexpected error occurred.",
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }

        // ======================
        //       VIEW DETAIL
        // ======================
        [HttpGet("detail/{testOrderId:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> GetDetail(
            Guid testOrderId, CancellationToken ct)
        {
            var result = await _mediator.Send(new ViewPatientTestOrderDetailQuery(testOrderId), ct);

            if (result.TestOrder == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = 404,
                    Message = result.Message,
                    ResponsedAt = DateTime.UtcNow
                });
            }

            return Ok(new ApiResponse<ViewPatientTestOrderDetailResult>
            {
                StatusCode = 200,
                Message = result.Message,
                Data = result,
                ResponsedAt = DateTime.UtcNow
            });
        }

        // ======================
        //       VIEW ALL
        // ======================
        [HttpGet("viewAll")]
        public async Task<ActionResult<ApiResponse<ViewPatientTestOrdersResult>>> ViewAll()
        {
            var result = await _mediator.Send(new ViewPatientTestOrdersQuery());

            return Ok(new ApiResponse<ViewPatientTestOrdersResult>
            {
                StatusCode = 200,
                Message = result.Items.Count == 0 ? "No Data" : "Success",
                Data = result,
                ResponsedAt = DateTime.UtcNow
            });
        }

        // ======================
        //        REVIEW
        // ======================
        [HttpPatch("review/{testOrderId:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> ReviewTestOrder(Guid testOrderId, CancellationToken ct)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new ApiResponse<bool>
                    {
                        StatusCode = 401,
                        Message = "User is not authenticated.",
                        ResponsedAt = DateTime.UtcNow
                    });
                }

                var command = new ReviewTestOrderCommand
                {
                    TestOrderId = testOrderId,
                    ReviewedBy = Guid.Parse(userId)
                };

                var success = await _mediator.Send(command, ct);

                if (!success)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Test order not found or already deleted.",
                        ResponsedAt = DateTime.UtcNow
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    StatusCode = 200,
                    Message = "Test order reviewed successfully.",
                    Data = true,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    StatusCode = 500,
                    Message = "An unexpected error occurred.",
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }

        // ======================
        //        DELETE
        // ======================
        [HttpDelete("delete/{testOrderId:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTestOrder(
            Guid testOrderId, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized(new ApiResponse<bool>
                {
                    StatusCode = 401,
                    Message = "User is not authenticated.",
                    ResponsedAt = DateTime.UtcNow
                });
            }

            var command = new DeleteTestOrderCommand
            {
                TestOrderId = testOrderId,
                DeletedBy = userId        // ?? Không còn Unknown n?a
            };

            var success = await _mediator.Send(command, ct);

            if (!success)
            {
                return NotFound(new ApiResponse<bool>
                {
                    StatusCode = 404,
                    Message = "Test order not found or already deleted.",
                    ResponsedAt = DateTime.UtcNow
                });
            }

            return Ok(new ApiResponse<bool>
            {
                StatusCode = 200,
                Message = "Test order deleted successfully.",
                Data = true,
                ResponsedAt = DateTime.UtcNow
            });
        }

        // ======================
        //        MODIFY
        // ======================
        [HttpPatch("modify/{testOrderId:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> ModifyPatientTestOrder(
            Guid testOrderId, [FromBody] ModifyPatientTestOrderCommand command)
        {
            try
            {
                command.TestOrderId = testOrderId;

                var result = await _mediator.Send(command);

                return Ok(new ApiResponse<bool>
                {
                    StatusCode = 200,
                    Message = "Patient's test order updated successfully with audit log.",
                    Data = result,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = 404,
                    Message = ex.Message,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = ex.Message,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = ex.Message,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    StatusCode = 500,
                    Message = $"An error occurred: {ex.Message}",
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }
    }
}
