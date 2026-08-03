namespace Events.Services.Helpers;

public static class StringTruncationHelper
{
    public static string TruncateString(string? input, int maxLength)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        return input.Length <= maxLength ? input : input[..maxLength];
    }
}
