using System.ComponentModel.DataAnnotations;

namespace AI_Solutions.Portal.Web.Models.ViewModels;

public class InquiryEditViewModel
{
    public int InquiryId { get; set; }
    public int CustomerId { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    [Display(Name = "Company Name")]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Country { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [Display(Name = "Job Title")]
    public string JobTitle { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Job Category")]
    public string JobCategory { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Job Details")]
    public string JobDetails { get; set; } = string.Empty;

    [StringLength(50)]
    public string Status { get; set; } = "New";
}
