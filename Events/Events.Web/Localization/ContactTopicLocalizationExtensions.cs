using Events.Web.Models;
using Events.Web.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace Events.Web.Localization;

/// <summary>
/// Extension methods that translate ContactTopic using the shared IStringLocalizer.
/// Resource keys follow the pattern: ContactTopic_{EnumName}, e.g. ContactTopic_AddEvent.
/// </summary>
public static class ContactTopicLocalizationExtensions
{
    public static string Localize(this ContactTopic topic, IStringLocalizer<SharedResources> localizer)
        => localizer[$"ContactTopic_{topic}"];

    public static List<SelectListItem> GetSelectListItems(IStringLocalizer<SharedResources> localizer) =>
        Enum.GetValues<ContactTopic>()
            .Select(topic => new SelectListItem
            {
                Value = topic.ToString(),
                Text = topic.Localize(localizer)
            })
            .ToList();
}
