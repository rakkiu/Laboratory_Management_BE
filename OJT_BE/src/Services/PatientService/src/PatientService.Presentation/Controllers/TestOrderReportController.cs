using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.UseCases.TestOrderUC.ExportExcel;
using PatientService.Application.UseCases.TestOrderUC.PrintPDF;

namespace PatientService.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestOrderReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TestOrderReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Export all test orders of a specific patient to Excel.
        /// </summary>
        /// <param name="patientId">The ID of the patient whose test orders will be exported.</param>
        /// <returns>Excel file containing the patient’s test orders.</returns>
        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportExcel([FromQuery] Guid? patientId)
        {
            var result = await _mediator.Send(new ExportTestOrdersQuery(patientId));

            return File(
                result.FileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                result.FileName
            );
        }



        /// <summary>
        /// Print a single test order report as PDF (only for Completed orders).
        /// </summary>
        /// <param name="id">The ID of the test order to print.</param>
        /// <returns>PDF file containing test order details and results.</returns>
        [HttpGet("print-pdf/{id:guid}")]
        public async Task<IActionResult> PrintPdf(Guid id, [FromQuery] string? fileName)
        {
            var result = await _mediator.Send(new PrintTestOrderQuery(id, fileName));

            return File(
                result.FileBytes,
                "application/pdf",
                result.FileName
            );
        }


    }
}
