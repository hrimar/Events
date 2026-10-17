using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Events.Services.Time;

public static class SofiaClockServiceCollectionExtensions
{
    // Shared by Events.Web and Events.Crawler, so both hosts resolve "today" the same way.
    // Required by EventService and VenueService.
    public static IServiceCollection AddSofiaClock(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<ISofiaClock, SofiaClock>();
        return services;
    }
}
