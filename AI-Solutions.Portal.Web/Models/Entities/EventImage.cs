using System.ComponentModel.DataAnnotations;

namespace AI_Solutions.Portal.Web.Models.Entities;

public class EventImage
{
    public int EventImageId { get; set; }

    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    [Required]
    [StringLength(500)]
    public string ImagePath { get; set; } = string.Empty;

    [StringLength(200)]
    public string Caption { get; set; } = string.Empty;
}
