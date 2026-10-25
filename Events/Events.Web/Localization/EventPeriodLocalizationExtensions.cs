using Events.Models.Enums;
using Events.Web.Resources;
using Microsoft.Extensions.Localization;

namespace Events.Web.Localization;

// Translates EventPeriod with the shared localizer. Resource keys: Period_{EnumName}, e.g. Period_Next7.
public static class EventPeriodLocalizationExtensions
{
    public static string Localize(this EventPeriod period, IStringLocalizer<SharedResources> localizer)
        => localizer[$"Period_{period}"];
}
