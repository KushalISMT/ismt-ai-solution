using System.ComponentModel.DataAnnotations;

namespace AI_Solutions.Portal.Web.Models.Entities;

public class Inquiry
{
    public int InquiryId { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string JobTitle { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string JobCategory { get; set; } = string.Empty;

    [Required]
    public string JobDetails { get; set; } = string.Empty;

    [StringLength(50)]
    public string Status { get; set; } = "New";

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
