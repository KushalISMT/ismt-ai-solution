using System.ComponentModel.DataAnnotations;

namespace AI_Solutions.Portal.Web.Models.Entities;

public class Solution
{
    public int SolutionId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [StringLength(100)]
    public string Industry { get; set; } = string.Empty;

    [StringLength(500)]
    public string Technologies { get; set; } = string.Empty;

    [StringLength(500)]
    public string? ImagePath { get; set; }

    public bool IsFeatured { get; set; }

    public ICollection<CaseStudy> CaseStudies { get; set; } = new List<CaseStudy>();
}
