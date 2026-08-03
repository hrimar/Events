using Events.Services.Import.Models;

namespace Events.Services.Interfaces;

/// <summary>
/// Maps a single raw row from an uploaded import file to a typed, validated <see cref="ImportRowResult"/>,
/// resolving free-text Category/Subcategory/Tags/Status against the database and flagging anything
/// that can't be resolved automatically for admin review, rather than guessing or auto-creating data.
/// </summary>
public interface IEventImportRowMapper
{
    Task<ImportRowResult> MapRowAsync(RawImportRow row, EventImportColumnMap columnMap, CancellationToken cancellationToken = default);
}
