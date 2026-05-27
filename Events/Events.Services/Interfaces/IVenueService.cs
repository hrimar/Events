using Events.Models.Entities;

namespace Events.Services.Interfaces;

public interface IVenueService
{
    Task<CanonicalVenue?> GetByIdAsync(int id);
    Task<CanonicalVenue?> GetBySlugAsync(string slug);
    Task<IEnumerable<CanonicalVenue>> GetAllAsync();
    Task<CanonicalVenue> CreateAsync(CanonicalVenue venue);
    Task<CanonicalVenue> UpdateAsync(CanonicalVenue venue);

    // Normalizes the raw location string and looks up a matching canonical venue.
    // Returns the canonical venue ID if found, or null if no alias match exists.
    Task<int?> FindCanonicalVenueIdAsync(string? rawLocation);

    // Generates a unique URL-friendly slug from a venue name.
    // Used as auto-generated value when admin creates a new venue — can be edited manually.
    Task<string> GenerateUniqueSlugAsync(string name);

    // Normalizes the raw alias string and adds it as a VenueAlias for the given venue.
    Task<VenueAlias> AddAliasAsync(int venueId, string rawAliasString);
}
