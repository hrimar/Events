using Events.Services.Import.Models;

namespace Events.Services.Interfaces;

/// <summary>
/// Checks an import row against already-saved events (and other rows in the same file) for
/// likely duplicates. Never skips a row silently — callers surface a match as a Warning so the
/// admin decides whether to exclude it.
/// </summary>
public interface IEventImportDuplicateDetector
{
    Task<(bool IsDuplicate, int? ExistingEventId)> FindExistingDuplicateAsync(ImportRowResult row, CancellationToken cancellationToken = default);

    /// <summary>
    /// Flags rows within the same batch that share the same Name+Date as an earlier row.
    /// </summary>
    void DetectIntraBatchDuplicates(IReadOnlyList<ImportRowResult> rows);
}
