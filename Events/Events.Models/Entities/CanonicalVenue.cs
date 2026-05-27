using System.ComponentModel.DataAnnotations;

namespace Events.Models.Entities;

public class CanonicalVenue
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? ShortName { get; set; }

    [Required]
    [MaxLength(100)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Address { get; set; }

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? PhotoUrl { get; set; }

    [MaxLength(500)]
    public string? WebsiteUrl { get; set; }

    public int? Capacity { get; set; }

    public DateTime CreatedAt { get; set; } // EventsDbContext will set this with .HasDefaultValueSql("GETUTCDATE()") to UtcNow when adding a new entity

    public DateTime UpdatedAt { get; set; } // EventsDbContext will set this with .HasDefaultValueSql("GETUTCDATE()") to UtcNow when adding a new entity

    public ICollection<VenueAlias> Aliases { get; set; } = new List<VenueAlias>();

    public ICollection<Event> Events { get; set; } = new List<Event>();
}
