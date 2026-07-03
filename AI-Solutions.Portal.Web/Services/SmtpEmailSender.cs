using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using AI_Solutions.Portal.Web.Models;

namespace AI_Solutions.Portal.Web.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<SmtpSettings> options, ILogger<SmtpEmailSender> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var fromAddress = new MailAddress(_settings.FromEmail, _settings.FromName);
        var toAddress = new MailAddress(email);

        using var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        // Test mode: save emails to a local folder instead of sending over SMTP
        if (!string.IsNullOrWhiteSpace(_settings.PickupDirectory))
        {
            Directory.CreateDirectory(_settings.PickupDirectory);

            using var client = new SmtpClient("localhost")
            {
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = _settings.PickupDirectory,
                EnableSsl = false
            };

            await client.SendMailAsync(message);
            _logger.LogInformation("Email saved to pickup directory for {Email}", email);
            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.Host) ||
            string.IsNullOrWhiteSpace(_settings.Username) ||
            string.IsNullOrWhiteSpace(_settings.Password))
        {
            _logger.LogWarning("SMTP settings are not configured. Email to {Email} was not sent.", email);
            throw new InvalidOperationException("SMTP settings are not configured. Please check appsettings.json.");
        }

        // Gmail app passwords are displayed with spaces for readability, but SMTP expects the 16 chars without them.
        var password = _settings.Password?.Replace(" ", string.Empty) ?? string.Empty;

        using var smtpClient = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.Username, password),
            EnableSsl = _settings.EnableSsl
        };

        await smtpClient.SendMailAsync(message);
        _logger.LogInformation("Email sent successfully to {Email}", email);
    }
}
