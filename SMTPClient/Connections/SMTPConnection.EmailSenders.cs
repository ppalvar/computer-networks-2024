using EmailClient.Common;
using EmailClient.Exceptions;
using EmailClient.Extensions;

namespace EmailClient.Connections;

internal partial class SMTPConnection
{
    public async Task SendEmail(EmailMessage message) =>
        await SendEmail(message.From, message.To, message.Subject, message.Body, message.Attachments.Count() == 0 ? null : message.Attachments.ToArray());
    public async Task SendEmail(string from, string to, string subject, string body, params FileAttachment[]? files) =>
        await SendEmail(from, new[] { to }, subject, body, files);

    public async Task SendEmail(string from, string[] to, string subject, string body, params FileAttachment[]? files)
    {
        string response;

        await Send(SMTPCommandBuilder.MAIL_FROM(from));
        response = await Receive();
        if (!GetCode(response).InValidCodes(SMTPResponseCode.OK))
            throw new InvalidServerResponse((int)GetCode(response));

        foreach (var recipient in to)
        {
            await Send(SMTPCommandBuilder.RCPT_TO(recipient));

            response = await Receive();
            if (!GetCode(response).InValidCodes(SMTPResponseCode.OK, SMTPResponseCode.UserNotLocalWillForward))
                throw new InvalidServerResponse((int)GetCode(response));
        }

        await Send(SMTPCommandBuilder.DATA());
        response = await Receive();
        if (!GetCode(response).InValidCodes(SMTPResponseCode.OK, SMTPResponseCode.StartMailInput))
            throw new InvalidServerResponse((int)GetCode(response));

        string boundary = $"--boundary_0_{Guid.NewGuid()}";

        await Send($"MIME-Version: 1.0{CRLF}");
        await Send($"From: {from}{CRLF}");
        await Send($"To: {to.Merge()}{CRLF}");
        await Send($"Date: {DateTime.Now}{CRLF}");
        await Send($"Subject: {subject}{CRLF}");
        await Send($"Content-Type: multipart/mixed;{CRLF} boundary={boundary}{CRLF}");
        await Send(CRLF);
        await Send(CRLF);


        await Send($"--{boundary}{CRLF}");
        await Send($"Content-Type: text/plain; charset=UTF-8{CRLF}");
        await Send($"Content-Transfer-Encoding: 7bit{CRLF}");
        await Send(CRLF);
        await Send($"{body}{CRLF}");

        if (files is FileAttachment[] _files) await SendAttachedFiles(_files, boundary);

        await Send($"--{boundary}--{CRLF}");
        await Send($".{CRLF}");

        response = await Receive();
        if (!GetCode(response).InValidCodes(SMTPResponseCode.OK))
            throw new InvalidServerResponse((int)GetCode(response));
    }

    private async Task SendAttachedFiles(FileAttachment[] files, string boundary)
    {
        foreach (var file in files)
        {
            await Send($"--{boundary}{CRLF}");
            await Send($"Content-Type: application/octet-stream; name={file.FileName}{CRLF}");
            await Send($"Content-Transfer-Encoding: base64{CRLF}");
            await Send($"Content-Disposition: attachment{CRLF}");
            await Send(CRLF);
            await Send($"{file.FileBase64}{CRLF}");
            await Send(CRLF);
        }
    }
}