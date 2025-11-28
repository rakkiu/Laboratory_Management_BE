using IAMService.Application.Models.Email;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IAMService.Application.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IAMService.Application.Interfaces.IEmailService" />
    public class SmtpEmailService : IEmailService
    {
        /// <summary>
        /// The settings
        /// </summary>
        private readonly EmailSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpEmailService"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public SmtpEmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        /// <summary>
        /// Sends the asynchronous.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        public async Task SendAsync(string to, string subject, string body)
        {
            using var smtp = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.FromAddress, _settings.Password),
                EnableSsl = _settings.EnableSsl
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromAddress, _settings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            message.To.Add(to);
            await smtp.SendMailAsync(message);
        }

        public async Task SendWithAttachmentAsync(
    string to,
    string subject,
    string body,
    string attachmentFileName,
    Stream attachmentStream)
        {
            using var smtp = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.FromAddress, _settings.Password),
                EnableSsl = _settings.EnableSsl
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromAddress, _settings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            message.To.Add(to);

            // Đính kèm file
            attachmentStream.Position = 0;
            var attachment = new Attachment(attachmentStream, attachmentFileName);
            message.Attachments.Add(attachment);

            await smtp.SendMailAsync(message);
        }

    }
}
