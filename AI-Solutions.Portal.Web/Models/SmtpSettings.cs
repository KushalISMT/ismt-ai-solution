namespace AI_Solutions.Portal.Web.Models;

public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = "AI-Solutions";
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// Optional local folder path for testing email without an SMTP server.
    /// When set, emails are saved as .eml files instead of being sent over the network.
    /// </summary>
    public string? PickupDirectory { get; set; }
}
