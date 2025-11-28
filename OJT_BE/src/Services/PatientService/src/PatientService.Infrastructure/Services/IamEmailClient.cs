using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IamEmailClient : IIamEmailClient
{
    private readonly HttpClient _client;

    public IamEmailClient(HttpClient client)
    {
        _client = client;
    }

    public async Task SendEmailWithAttachmentAsync(
        string to, string subject, string body,
        byte[] fileBytes, string fileName,
        CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(to), "To");
        content.Add(new StringContent(subject), "Subject");
        content.Add(new StringContent(body), "Body");

        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

        content.Add(fileContent, "Attachment", fileName);

        var response = await _client.PostAsync(
            "/api/users/send-email-with-attachment",
            content,
            cancellationToken
        );


        response.EnsureSuccessStatusCode();
    }
}
