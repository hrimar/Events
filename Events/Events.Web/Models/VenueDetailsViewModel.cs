using System.Globalization;

namespace Events.Web.Models;

public class VenueDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string City { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Description { get; set; }
    public string? PhotoUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public int? Capacity { get; set; }
    public bool HasMap => Latitude.HasValue && Longitude.HasValue;
    public IReadOnlyList<EventViewModel> UpcomingEvents { get; set; } = Array.Empty<EventViewModel>();

    // Preformatted coordinate strings for OpenStreetMap embed URL (InvariantCulture, dot separator)
    public string LatStr => Latitude.HasValue
        ? Latitude.Value.ToString("F6", CultureInfo.InvariantCulture) : string.Empty;
    public string LonStr => Longitude.HasValue
        ? Longitude.Value.ToString("F6", CultureInfo.InvariantCulture) : string.Empty;
    public string BboxMinLat => Latitude.HasValue
        ? (Latitude.Value - 0.003m).ToString("F6", CultureInfo.InvariantCulture) : string.Empty;
    public string BboxMaxLat => Latitude.HasValue
        ? (Latitude.Value + 0.003m).ToString("F6", CultureInfo.InvariantCulture) : string.Empty;
    public string BboxMinLon => Longitude.HasValue
        ? (Longitude.Value - 0.005m).ToString("F6", CultureInfo.InvariantCulture) : string.Empty;
    public string BboxMaxLon => Longitude.HasValue
        ? (Longitude.Value + 0.005m).ToString("F6", CultureInfo.InvariantCulture) : string.Empty;
}
