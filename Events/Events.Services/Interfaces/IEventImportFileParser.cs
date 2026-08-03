using Events.Services.Import.Models;

namespace Events.Services.Interfaces;

/// <summary>
/// Parses an uploaded file of a specific format (e.g. .xlsx, .csv) into a format-agnostic
/// <see cref="RawImportSheet"/>. Register one implementation per supported format and resolve
/// the correct one via <see cref="IEventImportFileParserFactory"/>.
/// </summary>
public interface IEventImportFileParser
{
    /// <summary>
    /// Returns true if this parser can handle the given file, based on its extension.
    /// </summary>
    bool CanParse(string fileName);

    Task<RawImportSheet> ParseAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
}

/// <summary>
/// Picks the correct <see cref="IEventImportFileParser"/> for an uploaded file's format.
/// </summary>
public interface IEventImportFileParserFactory
{
    /// <exception cref="NotSupportedException">Thrown when no registered parser can handle the file.</exception>
    IEventImportFileParser GetParser(string fileName);
}
