using System.ComponentModel.DataAnnotations;

namespace AI_Solutions.Portal.Web.Models.Entities;

public class Feedback
{
    public int FeedbackId { get; set; }

    [Required]
    [StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [StringLength(150)]
    public string CompanyName { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; }

    public string? Comment { get; set; }

    public bool IsApproved { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
