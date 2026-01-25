namespace Events.Services.Helpers;

public static class TagNameNormalizer
{
    private const int MaxLength = 50;
    private static readonly string[] ForbiddenSubstrings = ["ул.", "№"];

    public static string? Normalize(string? rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
        {
            return null;
        }

        var cleaned = rawName.Trim();
        cleaned = cleaned.Replace(((char)13).ToString(), " ").Replace(((char)10).ToString(), " ");
        cleaned = string.Join(" ", cleaned.Split(new[] { ' ', (char)9 }, StringSplitOptions.RemoveEmptyEntries));

        if (cleaned.Length > MaxLength)
        {
            var sliceLength = Math.Max(0, MaxLength - 3);
            cleaned = cleaned[..sliceLength].TrimEnd();
            if (cleaned.Length > 0)
            {
                cleaned += "...";
            }
        }

        if (cleaned.Contains('(') || cleaned.Contains(')'))
        {
            return null;
        }

        if (ForbiddenSubstrings.Any(substring => cleaned.Contains(substring, StringComparison.OrdinalIgnoreCase)))
        {
            return null;
        }

        return cleaned;
    }

    public static bool IsValid(string? rawName) => !string.IsNullOrEmpty(Normalize(rawName));
}
