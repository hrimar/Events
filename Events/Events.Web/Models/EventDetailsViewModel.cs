using Events.Models.Entities;

namespace Events.Web.Models;

// Adds SEO-specific fields to EventViewModel for the individual event details page,
// reusing EventViewModel's entity mapping (see EventViewModel.Populate) instead of
// duplicating it. Mirrors the VenueDetailsViewModel.JsonLd pattern.
public class EventDetailsViewModel : EventViewModel
{
    public string JsonLd { get; set; } = string.Empty;

    public static EventDetailsViewModel FromEntity(Event eventEntity, string jsonLd)
    {
        var model = Populate(new EventDetailsViewModel(), eventEntity);
        model.JsonLd = jsonLd;
        return model;
    }
}
