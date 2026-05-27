using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Services.Helpers;
using Events.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Events.Services.Implementations;

public class VenueService : IVenueService
{
    private readonly IVenueRepository _repository;
    private readonly ILogger<VenueService> _logger;

    public VenueService(IVenueRepository repository, ILogger<VenueService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CanonicalVenue?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

    public async Task<CanonicalVenue?> GetBySlugAsync(string slug) => await _repository.GetBySlugAsync(slug);

    public async Task<IEnumerable<CanonicalVenue>> GetAllAsync() => await _repository.GetAllAsync();

    public async Task<CanonicalVenue> CreateAsync(CanonicalVenue venue) => await _repository.AddAsync(venue);

    public async Task<CanonicalVenue> UpdateAsync(CanonicalVenue venue) => await _repository.UpdateAsync(venue);

    public async Task<int?> FindCanonicalVenueIdAsync(string? rawLocation)
    {
        var normalized = VenueNormalizer.Normalize(rawLocation);

        if (normalized == null)
            return null;

        try
        {
            // Step 1: exact alias match — fast indexed lookup
            var venue = await _repository.FindByNormalizedAliasAsync(normalized);

            if (venue != null)
            {
                _logger.LogDebug("Exact alias match for '{RawLocation}' -> '{VenueName}' (Id={VenueId})",
                    rawLocation, venue.Name, venue.Id);
                return venue.Id;
            }

            // Step 2: contains match — check if normalized location contains any known venue name.
            // All venues are loaded in-memory (100-150 records max) — acceptable for this scale.
            var allVenues = await _repository.GetAllAsync();
            var containsMatch = allVenues.FirstOrDefault(v =>
            {
                var normalizedVenueName = VenueNormalizer.Normalize(v.Name);
                return normalizedVenueName != null && normalized.Contains(normalizedVenueName);
            });

            if (containsMatch != null)
            {
                _logger.LogInformation(
                    "Contains match for '{RawLocation}' -> '{VenueName}' (Id={VenueId}). Auto-creating alias.",
                    rawLocation, containsMatch.Name, containsMatch.Id);

                // Auto-create alias so the next crawl hits Step 1 directly
                await _repository.AddAliasAsync(new VenueAlias
                {
                    CanonicalVenueId = containsMatch.Id,
                    AliasString = rawLocation!.Trim(),
                    NormalizedString = normalized
                });

                return containsMatch.Id;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Venue lookup failed for location '{RawLocation}', event will be saved as unmapped", rawLocation);
            return null;
        }
    }

    // Generates a unique slug — auto-proposed when admin creates a venue, editable before saving.
    // Appends a numeric suffix only on the rare case of a collision (e.g. "ndk-2").
    public async Task<string> GenerateUniqueSlugAsync(string name)
    {
        var baseSlug = VenueNormalizer.GenerateSlug(name);

        if (string.IsNullOrEmpty(baseSlug))
            baseSlug = "venue";

        var slug = baseSlug;
        var suffix = 2;

        try
        {
            while (await _repository.SlugExistsAsync(slug))
            {
                slug = $"{baseSlug}-{suffix}";
                suffix++;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Slug uniqueness check failed for '{BaseSlug}', returning base slug", baseSlug);
        }

        return slug;
    }

    // Normalizes the raw alias string and persists it as a VenueAlias for the given venue.
    public async Task<VenueAlias> AddAliasAsync(int venueId, string rawAliasString)
    {
        var normalized = VenueNormalizer.Normalize(rawAliasString)
            ?? throw new ArgumentException($"Cannot normalize alias string: '{rawAliasString}'");

        var alias = new VenueAlias
        {
            CanonicalVenueId = venueId,
            AliasString = rawAliasString.Trim(),
            NormalizedString = normalized
        };

        return await _repository.AddAliasAsync(alias);
    }
}
