using Events.Services.Import.Models;

namespace Events.Services.Interfaces;

/// <summary>
/// Orchestrates the admin bulk-import flow: parse an uploaded file, map/validate every row, and
/// (after the admin reviews it in the Preview screen) commit the accepted rows to the database.
/// </summary>
public interface IEventImportService
{
    Task<EventImportBatch> ParseAndValidateAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);

    Task<EventImportCommitResult> CommitAsync(EventImportBatch batch, CancellationToken cancellationToken = default);
}
