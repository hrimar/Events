using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Events.Services.Media;

public static class MediaServiceCollectionExtensions
{
    // Named HttpClient with the SSRF-safe handler, used for Excel import images and pages (GS-REQ-009, 9.1, 9.2).
    public static IServiceCollection AddSafeHttpFetcher(this IServiceCollection services)
    {
        services.AddHttpClient(SafeHttpFetcher.HttpClientName, client => client.Timeout = Timeout.InfiniteTimeSpan)
            .ConfigurePrimaryHttpMessageHandler(SafeHttpFetcher.CreateHandler);

        services.TryAddSingleton<ISafeHttpFetcher, SafeHttpFetcher>();
        return services;
    }

    // Images stored by content with WebP variants (GS-REQ-009, 9.1.4, 9.4).
    // Requires a BlobContainerClient registration (the "event-images" container).
    public static IServiceCollection AddMediaImageStore(this IServiceCollection services)
    {
        services.TryAddSingleton<ImageVariantGenerator>();
        services.TryAddSingleton<IMediaBlobStorage, AzureMediaBlobStorage>();
        services.TryAddSingleton<IMediaImageStore, MediaImageStore>();
        return services;
    }
}
