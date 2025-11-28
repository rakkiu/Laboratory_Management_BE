using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Models;
using PatientService.Application.Models.PatientDto;
using PatientService.Application.Models.PatientMedicalRecordDto;
using PatientService.Application.UseCases.MedicalRecord.Commands.CreateMedicalRecord;
using PatientService.Application.UseCases.MedicalRecord.Commands.DeleteMedicalRecord;
using PatientService.Application.UseCases.MedicalRecord.Commands.UpdateMedicalRecord;
using PatientService.Application.UseCases.MedicalRecord.Queries.GetAllMedicalRecord;
using PatientService.Application.UseCases.MedicalRecord.Queries.ViewMedicalRecordDetail;
using PatientService.Application.UseCases.MedicalRecord.Commands.DeletePatient;
using System;
using System.Threading.Tasks;

namespace PatientService.Presentation.Controllers
{
    /// <summary>
    /// medical record controller
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/medicalrecord")]
    [ApiController]
    public class PatientMedicalRecordController : ControllerBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;
        /// <summary>
        /// Initializes a new instance of the <see cref="PatientMedicalRecordController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public PatientMedicalRecordController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates the medical record.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<CreatePatientMedicalRecord>>> CreateMedicalRecord([FromBody] CreatePatientMedicalRecordCommad command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(new ApiResponse<CreatePatientMedicalRecord>
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Medical record created successfully with audit log.",
                    Data = result,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets all medical records.
        /// </summary>
        /// <returns></returns>
        [HttpGet("getall")]
        public async Task<ActionResult<ApiResponse<List<MedicalRecordDto>>>> GetAllMedicalRecords()
        {
            try
            {
                var query = new GetAllMedicalRecordsQuery();
                var result = await _mediator.Send(query);
                return Ok(new ApiResponse<List<ListPatientDto?>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Medical records retrieved successfully.",
                    Data = result,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Updates the medical record.
        /// </summary>
        /// <param name="id">The medical record identifier.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>

        [HttpPatch("update/by-patient/{patientId}")]
        public async Task<ActionResult<ApiResponse<MedicalRecordDto>>> UpdateMedicalRecordByPatient(Guid patientId, [FromBody] UpdatePatientMedicalRecordCommand command)
        {
            try
            {
                // Set the PatientId in the command from the route parameter
                command.PatientId = patientId;

                var result = await _mediator.Send(command);
                return Ok(new ApiResponse<MedicalRecordDto>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Patient and latest medical record updated successfully.",
                    Data = result,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }


        [HttpGet("detail/by-patient/{patientId}")]
        public async Task<ActionResult<ApiResponse<ViewMedicalRecordDetailResponse>>> GetMedicalRecordDetailByPatient(Guid patientId)
        {
            try
            {
                var query = new ViewMedicalRecordDetailQuery
                {
                    PatientId = patientId
                };
                var result = await _mediator.Send(query);
                return Ok(new ApiResponse<ViewMedicalRecordDetailResponse>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Medical record detail retrieved successfully.",
                    Data = result,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    Data = null,
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }
    

        /// <summary>
        /// Deletes all medical records for a patient (soft delete).
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="deletedBy">The user who performed the deletion.</param>
        /// <returns></returns>
        [HttpDelete("delete/{patientId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteMedicalRecordsByPatient(Guid patientId, [FromQuery] Guid deletedBy)
        {
            try
            {
                var command = new DeleteMedicalRecordCommand
                {
                    PatientId = patientId,
                    DeletedBy = deletedBy
                };

                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = $"No medical records found for patient with ID {patientId}.",
                        Data = false,
                        ResponsedAt = DateTime.UtcNow
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Medical records for the patient deleted successfully with audit log.",
                    Data = true,
                    ResponsedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while deleting the medical records.",
                    Data = ex.Message,
                    ResponsedAt = DateTime.UtcNow
                });
            }
        }

  
    }
}
