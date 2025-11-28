using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IIamEmailClient
{
    Task SendEmailWithAttachmentAsync(
        string to,
        string subject,
        string body,
        byte[] fileBytes,
        string fileName,
        CancellationToken cancellationToken = default
    );
}
