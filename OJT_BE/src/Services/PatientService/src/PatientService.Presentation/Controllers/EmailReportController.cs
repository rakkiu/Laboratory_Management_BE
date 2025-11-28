using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Interfaces;
using PatientService.Application.UseCases.TestOrderUC.PrintPDF;
using PatientService.Application.UseCases.TestOrderUC.GetEmailInfo;

namespace PatientService.Presentation.Controllers
{
    [ApiController]
    [Route("api/reports/email")]
    public class EmailReportController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IIamEmailClient _emailClient;

        public EmailReportController(
            IMediator mediator,
            IIamEmailClient emailClient)
        {
            _mediator = mediator;
            _emailClient = emailClient;
        }

        /// <summary>
        /// Generate TestOrder PDF and send to patient email
        /// </summary>
        [HttpPost("send-testorder/{id:guid}")]
        public async Task<IActionResult> SendTestOrderReport(Guid id)
        {
            // 1. Get info: patient name + email
            var emailInfo = await _mediator.Send(new GetTestOrderEmailInfoQuery(id));

            if (string.IsNullOrWhiteSpace(emailInfo.Email))
                return BadRequest(new { message = "Patient does not have an email." });

            // 2. Generate PDF
            var pdf = await _mediator.Send(new PrintTestOrderQuery(id, null));

            // 3. Build email body
            string body = $@"
Kính gửi: {emailInfo.PatientName}

Mã đơn xét nghiệm: {emailInfo.TestOrderId}

Đây là phiếu kết quả xét nghiệm của bạn. 
Vui lòng kiểm tra file PDF đính kèm.

Trân trọng,
Trung tâm xét nghiệm
";

            // 4. Send email
            await _emailClient.SendEmailWithAttachmentAsync(
                to: emailInfo.Email,
                subject: "Kết quả xét nghiệm của bạn",
                body: body,
                fileBytes: pdf.FileBytes,
                fileName: pdf.FileName
            );

            return Ok(new { message = "Email sent successfully" });
        }
    }
}
