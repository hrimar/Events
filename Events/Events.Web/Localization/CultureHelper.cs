using System.Globalization;

namespace Events.Web.Localization;

public static class CultureHelper
{
    public static bool IsEnglish() =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("en", StringComparison.OrdinalIgnoreCase);

    // Numeric date format shown to visitors: BG dd.MM.yyyy, EN dd/MM/yyyy (GS-BUG-004).
    public static string ShortDateFormat(bool isEnglish) => isEnglish ? "dd/MM/yyyy" : "dd.MM.yyyy";
}
