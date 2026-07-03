using System.ComponentModel.DataAnnotations;
using AI_Solutions.Portal.Web.Models.Validation;

namespace AI_Solutions.Portal.Web.Models.ViewModels;

public class ContactFormViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 7)]
    [RegularExpression(@"^\+?[\d\s\-\(\)]{7,50}$",
        ErrorMessage = "Please enter a valid phone number with at least 7 digits (optional +, spaces, dashes, brackets allowed).")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Company Name")]
    [StringLength(150)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Country]
    public string Country { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Job Title")]
    [StringLength(200)]
    public string JobTitle { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Job Category")]
    [StringLength(100)]
    public string JobCategory { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Job Details")]
    public string JobDetails { get; set; } = string.Empty;
}
