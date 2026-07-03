using System.ComponentModel.DataAnnotations;

namespace AI_Solutions.Portal.Web.Models.ViewModels;

public class FeedbackFormViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Your Name")]
    public string CustomerName { get; set; } = string.Empty;

    [StringLength(150)]
    [Display(Name = "Company Name")]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [Range(1, 5, ErrorMessage = "Please select a rating between 1 and 5.")]
    [Display(Name = "Rating")]
    public int Rating { get; set; }

    [Display(Name = "Your Feedback")]
    public string? Comment { get; set; }
}
