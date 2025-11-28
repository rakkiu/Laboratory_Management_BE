using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PatientService.Application.Models.PatientMedicalRecordDto;


namespace PatientService.Presentation.Filters
{
    /// <summary>
    /// Filter to handle FluentValidation exceptions and return ApiResponse format
    /// </summary>
    public class ValidationExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ValidationException validationException)
            {
                var errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                var errorMessage = string.Join("; ", validationException.Errors.Select(e => e.ErrorMessage));

                var response = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = $"Validation failed: {errorMessage}",
                    Data = errors,
                    ResponsedAt = DateTime.UtcNow
                };

                context.Result = new BadRequestObjectResult(response);
                context.ExceptionHandled = true;
            }
        }
    }
}
