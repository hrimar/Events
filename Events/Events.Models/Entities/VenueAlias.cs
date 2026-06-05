using System.ComponentModel.DataAnnotations;

namespace Events.Models.Entities;

public class VenueAlias
{
    public int Id { get; set; }

    [Required]
    [MaxLength(300)]
    public string AliasString { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string NormalizedString { get; set; } = string.Empty;

    [Required]
    public int CanonicalVenueId { get; set; }

    public CanonicalVenue CanonicalVenue { get; set; } = null!;

    public DateTime CreatedAt { get; set; } // EventsDbContext will set this with .HasDefaultValueSql("GETUTCDATE()") to UtcNow when adding a new entity
}
