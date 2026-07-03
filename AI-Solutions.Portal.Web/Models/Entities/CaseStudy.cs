using System.ComponentModel.DataAnnotations;

namespace AI_Solutions.Portal.Web.Models.Entities;

public class CaseStudy
{
    public int CaseStudyId { get; set; }

    public int SolutionId { get; set; }
    public Solution Solution { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [StringLength(150)]
    public string ClientName { get; set; } = string.Empty;

    [StringLength(100)]
    public string Industry { get; set; } = string.Empty;

    public string Outcome { get; set; } = string.Empty;

    [StringLength(500)]
    public string? ImagePath { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
