using System.Text.Json;

namespace Events.Web.Infrastructure.JsonLd;

// Small helper around a schema.org JSON-LD object, so builders only ever add a field
// when there is real data for it - never a guessed/fabricated value that Google could
// flag as incorrect structured data.
public sealed class SafeJsonLdBuilder
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = false };

    private readonly Dictionary<string, object?> _fields = new();

    public SafeJsonLdBuilder Add(string key, object? value)
    {
        _fields[key] = value;
        return this;
    }

    public SafeJsonLdBuilder AddIfNotNull(string key, object? value)
    {
        if (value != null)
            _fields[key] = value;

        return this;
    }

    public SafeJsonLdBuilder AddIfNotEmpty(string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            _fields[key] = value;

        return this;
    }

    public Dictionary<string, object?> Build() => _fields;

    public static string Serialize(object value) => JsonSerializer.Serialize(value, SerializerOptions);

    // Wraps multiple schema.org objects (built with includeContext: false) into a single
    // "@graph" document, so a page can emit one <script type="application/ld+json">
    // instead of one per schema type.
    public static Dictionary<string, object?> BuildGraph(params object[] nodes)
    {
        return new SafeJsonLdBuilder()
            .Add("@context", "https://schema.org")
            .Add("@graph", nodes)
            .Build();
    }
}
