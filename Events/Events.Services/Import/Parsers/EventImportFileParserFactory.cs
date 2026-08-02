using Events.Services.Interfaces;

namespace Events.Services.Import.Parsers;

public class EventImportFileParserFactory : IEventImportFileParserFactory
{
    private readonly IEnumerable<IEventImportFileParser> _parsers;

    public EventImportFileParserFactory(IEnumerable<IEventImportFileParser> parsers)
    {
        _parsers = parsers;
    }

    public IEventImportFileParser GetParser(string fileName)
    {
        var parser = _parsers.FirstOrDefault(p => p.CanParse(fileName));
        if (parser == null)
        {
            throw new NotSupportedException($"No import parser registered for file '{fileName}'. Supported formats: .xlsx, .csv");
        }

        return parser;
    }
}
