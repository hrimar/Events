using System.Globalization;

namespace Events.Web.Localization;

public static class CultureHelper
{
    public static bool IsEnglish() =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("en", StringComparison.OrdinalIgnoreCase);
}
