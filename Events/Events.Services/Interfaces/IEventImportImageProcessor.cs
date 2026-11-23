using Events.Models.Entities;

namespace Events.Services.Interfaces;

// Copies the image of one imported row into our storage and links it to the event (GS-REQ-009, 9.1, 9.2, 9.3.1).
public interface IEventImportImageProcessor
{
    // Processes a Pending item: fills Outcome, FailureReason, Detail, ImageFromPage and ProcessedAtUtc, and on success
    // sets the event image. Never throws except on cancellation; the caller saves the item.
    Task ProcessAsync(EventImportJobItem item, CancellationToken cancellationToken = default);
}
