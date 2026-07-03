using System.ComponentModel.DataAnnotations;

namespace AI_Solutions.Portal.Web.Models.Entities;

public class Event
{
    public int EventId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public DateTime EventDate { get; set; }

    [StringLength(200)]
    public string Location { get; set; } = string.Empty;

    [StringLength(50)]
    public string EventType { get; set; } = "Upcoming"; // Upcoming or Past

    [StringLength(500)]
    public string? ImagePath { get; set; }

    public ICollection<EventImage> EventImages { get; set; } = new List<EventImage>();
}
