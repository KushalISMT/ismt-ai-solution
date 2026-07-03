using System.ComponentModel.DataAnnotations;

namespace AI_Solutions.Portal.Web.Models.ViewModels;

public class SendEmailViewModel
{
    public int InquiryId { get; set; }

    [Required]
    [EmailAddress]
    public string ToEmail { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Customer Name")]
    public string ToName { get; set; } = string.Empty;

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Message")]
    public string Body { get; set; } = string.Empty;
}
