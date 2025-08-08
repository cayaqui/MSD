using Application.Interfaces.Notifications;
using Core.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using System.Net;

namespace Infrastructure.Services.Notifications;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly ISendGridClient _sendGridClient;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly bool _useSendGrid;

    public EmailService(
        IConfiguration configuration,
        ILogger<EmailService> logger,
        ISendGridClient sendGridClient)
    {
        _configuration = configuration;
        _logger = logger;
        _sendGridClient = sendGridClient;
        
        _fromEmail = _configuration["Email:FromEmail"] ?? "noreply@ezpro.com";
        _fromName = _configuration["Email:FromName"] ?? "EzPro System";
        _useSendGrid = _configuration.GetValue<bool>("Email:UseSendGrid");
    }

    public async Task<Result> SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = false,
        CancellationToken cancellationToken = default)
    {
        return await SendEmailAsync(
            new List<string> { to },
            subject,
            body,
            isHtml,
            null,
            null,
            cancellationToken);
    }

    public async Task<Result> SendEmailAsync(
        List<string> to,
        string subject,
        string body,
        bool isHtml = false,
        List<string>? cc = null,
        List<string>? bcc = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (_useSendGrid)
            {
                return await SendViaSendGrid(to, subject, body, isHtml, cc, bcc, cancellationToken);
            }
            else
            {
                return await SendViaSmtp(to, subject, body, isHtml, cc, bcc, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email");
            return Result.Failure($"Failed to send email: {ex.Message}");
        }
    }

    public async Task<Result> SendEmailWithAttachmentsAsync(
        string to,
        string subject,
        string body,
        List<EmailAttachment> attachments,
        bool isHtml = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (_useSendGrid)
            {
                var msg = new SendGridMessage
                {
                    From = new EmailAddress(_fromEmail, _fromName),
                    Subject = subject
                };

                msg.AddTo(new EmailAddress(to));

                if (isHtml)
                    msg.HtmlContent = body;
                else
                    msg.PlainTextContent = body;

                foreach (var attachment in attachments)
                {
                    msg.AddAttachment(
                        attachment.FileName,
                        Convert.ToBase64String(attachment.Content),
                        attachment.ContentType);
                }

                var response = await _sendGridClient.SendEmailAsync(msg, cancellationToken);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted ||
                    response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Result.Success();
                }

                var responseBody = await response.Body.ReadAsStringAsync();
                return Result.Failure($"SendGrid error: {response.StatusCode} - {responseBody}");
            }
            else
            {
                using var mail = new MailMessage();
                mail.From = new MailAddress(_fromEmail, _fromName);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = isHtml;

                foreach (var attachment in attachments)
                {
                    var stream = new MemoryStream(attachment.Content);
                    mail.Attachments.Add(new Attachment(stream, attachment.FileName, attachment.ContentType));
                }

                using var smtp = GetSmtpClient();
                await smtp.SendMailAsync(mail, cancellationToken);

                return Result.Success();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email with attachments");
            return Result.Failure($"Failed to send email: {ex.Message}");
        }
    }

    public async Task<Result> SendTemplatedEmailAsync(
        string to,
        string templateId,
        Dictionary<string, string> templateData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_useSendGrid)
            {
                return Result.Failure("Templated emails require SendGrid configuration");
            }

            var msg = new SendGridMessage
            {
                From = new EmailAddress(_fromEmail, _fromName),
                TemplateId = templateId
            };

            msg.AddTo(new EmailAddress(to));
            msg.SetTemplateData(templateData);

            var response = await _sendGridClient.SendEmailAsync(msg, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted ||
                response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Result.Success();
            }

            var responseBody = await response.Body.ReadAsStringAsync();
            return Result.Failure($"SendGrid error: {response.StatusCode} - {responseBody}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send templated email");
            return Result.Failure($"Failed to send email: {ex.Message}");
        }
    }

    public async Task<Result<BulkEmailResult>> SendBulkEmailsAsync(
        List<BulkEmailRequest> requests,
        CancellationToken cancellationToken = default)
    {
        var result = new BulkEmailResult
        {
            TotalRequests = requests.Count
        };

        foreach (var request in requests)
        {
            try
            {
                var sendResult = await SendEmailAsync(
                    request.To,
                    request.Subject,
                    request.Body,
                    request.IsHtml,
                    cancellationToken);

                if (sendResult.IsSuccess)
                {
                    result.SuccessfulSends++;
                }
                else
                {
                    result.FailedSends++;
                    result.Errors.Add($"Failed to send to {request.To}: {sendResult.Message}");
                }
            }
            catch (Exception ex)
            {
                result.FailedSends++;
                result.Errors.Add($"Failed to send to {request.To}: {ex.Message}");
            }
        }

        return Result<BulkEmailResult>.Success(result);
    }

    private async Task<Result> SendViaSendGrid(
        List<string> to,
        string subject,
        string body,
        bool isHtml,
        List<string>? cc,
        List<string>? bcc,
        CancellationToken cancellationToken)
    {
        var msg = new SendGridMessage
        {
            From = new EmailAddress(_fromEmail, _fromName),
            Subject = subject
        };

        foreach (var recipient in to)
        {
            msg.AddTo(new EmailAddress(recipient));
        }

        if (cc != null)
        {
            foreach (var recipient in cc)
            {
                msg.AddCc(new EmailAddress(recipient));
            }
        }

        if (bcc != null)
        {
            foreach (var recipient in bcc)
            {
                msg.AddBcc(new EmailAddress(recipient));
            }
        }

        if (isHtml)
            msg.HtmlContent = body;
        else
            msg.PlainTextContent = body;

        var response = await _sendGridClient.SendEmailAsync(msg, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Accepted ||
            response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return Result.Success();
        }

        var responseBody = await response.Body.ReadAsStringAsync();
        return Result.Failure($"SendGrid error: {response.StatusCode} - {responseBody}");
    }

    private async Task<Result> SendViaSmtp(
        List<string> to,
        string subject,
        string body,
        bool isHtml,
        List<string>? cc,
        List<string>? bcc,
        CancellationToken cancellationToken)
    {
        using var mail = new MailMessage();
        mail.From = new MailAddress(_fromEmail, _fromName);
        
        foreach (var recipient in to)
        {
            mail.To.Add(recipient);
        }

        if (cc != null)
        {
            foreach (var recipient in cc)
            {
                mail.CC.Add(recipient);
            }
        }

        if (bcc != null)
        {
            foreach (var recipient in bcc)
            {
                mail.Bcc.Add(recipient);
            }
        }

        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = isHtml;

        using var smtp = GetSmtpClient();
        await smtp.SendMailAsync(mail, cancellationToken);

        return Result.Success();
    }

    private SmtpClient GetSmtpClient()
    {
        var smtp = new SmtpClient
        {
            Host = _configuration["Email:SmtpHost"] ?? "localhost",
            Port = _configuration.GetValue<int>("Email:SmtpPort", 587),
            EnableSsl = _configuration.GetValue<bool>("Email:EnableSsl", true),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false
        };

        var username = _configuration["Email:SmtpUsername"];
        var password = _configuration["Email:SmtpPassword"];

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            smtp.Credentials = new NetworkCredential(username, password);
        }

        return smtp;
    }
}