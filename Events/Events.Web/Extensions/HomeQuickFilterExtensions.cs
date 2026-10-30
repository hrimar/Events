using Events.Models.Entities;
using Events.Models.Enums;
using Events.Models.Queries;
using Events.Services.Time;
using Microsoft.AspNetCore.Routing;

namespace Events.Web.Extensions;

// Translates a homepage "What" chip into the "All events" filter it stands for (GS-REQ-007, 7.3),
// so the chip link and the "has events" check always use the same filter.
public static class HomeQuickFilterExtensions
{
    // Adds the chip's filter to a listing query.
    public static PublicEventQuery ApplyTo(this HomeQuickFilter filter, PublicEventQuery query) => filter.Type switch
    {
        HomeQuickFilterType.Categories => query with { CategoryNames = filter.Values },
        HomeQuickFilterType.Tags => query with { TagNames = filter.Values },
        HomeQuickFilterType.Free => query with { IsFree = true },
        _ => query
    };

    // Route values of the chip link: "All events" for the next 7 days with the chip's filter,
    // e.g. /Events?period=next7&category=Business,Conferences.
    public static RouteValueDictionary ToRouteValues(this HomeQuickFilter filter)
    {
        var route = new RouteValueDictionary
        {
            ["period"] = EventPeriodCalculator.ToQueryValue(EventPeriod.Next7)
        };

        switch (filter.Type)
        {
            case HomeQuickFilterType.Categories:
                route["category"] = string.Join(",", filter.Values);
                break;
            case HomeQuickFilterType.Tags:
                route["tags"] = string.Join(",", filter.Values);
                break;
            case HomeQuickFilterType.Free:
                route["free"] = "true";
                break;
        }

        return route;
    }
}
