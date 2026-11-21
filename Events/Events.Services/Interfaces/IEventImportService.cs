using Events.Services.Import.Models;

namespace Events.Services.Interfaces;

/// <summary>
/// Orchestrates the admin bulk-import flow: parse an uploaded file, map/validate every row, and
/// (after the admin reviews it in the Preview screen) commit the accepted rows to the database.
/// </summary>
public interface IEventImportService
{
    Task<EventImportBatch> ParseAndValidateAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);

    // Creates the events and saves the image job that copies their images in the background (GS-REQ-009, 9.3.4);
    // createdBy is the admin user name shown in the report.
    Task<EventImportCommitResult> CommitAsync(EventImportBatch batch, string createdBy, CancellationToken cancellationToken = default);
}
