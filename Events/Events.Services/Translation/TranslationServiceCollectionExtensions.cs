using Events.Data.Repositories.Implementations;
using Events.Data.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Events.Services.Translation;

public static class TranslationServiceCollectionExtensions
{
    private const string ClaudeApiKeySetting = "Claude:ApiKey";
    private const string ClaudeApiBaseAddress = "https://api.anthropic.com/v1/";
    private const string AnthropicVersion = "2023-06-01";

    // English translation of event texts (GS-REQ-010, 10.4). Used by the crawler; requires the EventsDbContext,
    // IEventRepository and ISofiaClock.
    public static IServiceCollection AddEventTranslation(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TranslationOptions>(configuration.GetSection(TranslationOptions.SectionName));
        services.PostConfigure<TranslationOptions>(options => options.ApiKey ??= configuration[ClaudeApiKeySetting]);

        services.AddHttpClient<ClaudeTranslationClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<TranslationOptions>>().Value;

            client.BaseAddress = new Uri(ClaudeApiBaseAddress);
            client.Timeout = TimeSpan.FromSeconds(options.RequestTimeoutSeconds);
            client.DefaultRequestHeaders.Add("anthropic-version", AnthropicVersion);

            if (!string.IsNullOrWhiteSpace(options.ApiKey))
                client.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
        });

        services.TryAddScoped<ITranslationCacheRepository, TranslationCacheRepository>();
        services.TryAddScoped<IEventTranslationService, EventTranslationService>();
        services.TryAddScoped<IUpcomingEventTranslator, UpcomingEventTranslator>();

        return services;
    }
}
