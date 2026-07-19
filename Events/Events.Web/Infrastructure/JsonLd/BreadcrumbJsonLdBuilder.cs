namespace Events.Web.Infrastructure.JsonLd;

// Builds a schema.org BreadcrumbList from an ordered trail of (Name, Url) pairs.
// The last item's Url is typically null - it represents the current page, which
// mirrors the visible <nav aria-label="breadcrumb"> markup where the active item
// has no link either.
public static class BreadcrumbJsonLdBuilder
{
    public static Dictionary<string, object?> BuildBreadcrumbList(IReadOnlyList<(string Name, string? Url)> items, bool includeContext = true)
    {
        var builder = new SafeJsonLdBuilder();

        if (includeContext)
            builder.Add("@context", "https://schema.org");

        builder.Add("@type", "BreadcrumbList");
        builder.Add("itemListElement", items.Select((item, index) =>
        {
            var listItem = new SafeJsonLdBuilder()
                .Add("@type", "ListItem")
                .Add("position", index + 1)
                .Add("name", item.Name);

            listItem.AddIfNotEmpty("item", item.Url);

            return listItem.Build();
        }).ToList());

        return builder.Build();
    }
}
