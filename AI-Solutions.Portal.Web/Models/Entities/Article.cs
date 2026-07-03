using System.ComponentModel.DataAnnotations;

namespace AI_Solutions.Portal.Web.Models.Entities;

public class Article
{
    public int ArticleId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(100)]
    public string Author { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [StringLength(500)]
    public string? FeaturedImagePath { get; set; }

    public DateTime PublishDate { get; set; } = DateTime.UtcNow;

    public bool IsPublished { get; set; } = true;
}
