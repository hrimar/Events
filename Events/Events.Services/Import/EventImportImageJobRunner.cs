using Events.Data.Repositories.Interfaces;
using Events.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Events.Services.Import;

// Runs one Excel import image job in the background (GS-REQ-009, 9.3.4): the Pending rows are processed one by one,
// each in its own DI scope (fresh DbContext) and saved as soon as it is done, so the report page shows progress.
// When the app stops, unprocessed rows stay Pending and the job stays Running; the worker picks it up again on the
// next start. Running a job twice is harmless: the second run finds no Pending rows and only completes it.
public sealed class EventImportImageJobRunner
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<EventImportImageJobRunner> _logger;

    public EventImportImageJobRunner(IServiceScopeFactory scopeFactory, TimeProvider timeProvider, ILogger<EventImportImageJobRunner> logger)
    {
        _scopeFactory = scopeFactory;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task RunAsync(int jobId, CancellationToken cancellationToken)
    {
        List<int> itemIds;
        await using (var scope = _scopeFactory.CreateAsyncScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IEventImportJobRepository>();
            if (!await repository.StartAsync(jobId, UtcNow(), cancellationToken))
            {
                _logger.LogInformation("Import image job {JobId} is missing or already completed", jobId);
                return;
            }

            itemIds = await repository.GetPendingItemIdsAsync(jobId, cancellationToken);
        }

        _logger.LogInformation("Import image job {JobId} started: {Count} row(s) to process", jobId, itemIds.Count);

        foreach (var itemId in itemIds)
        {
            await ProcessItemAsync(itemId, cancellationToken);
        }

        await using (var scope = _scopeFactory.CreateAsyncScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IEventImportJobRepository>();
            await repository.CompleteAsync(jobId, UtcNow(), cancellationToken);
        }

        _logger.LogInformation("Import image job {JobId} completed", jobId);
    }

    private async Task ProcessItemAsync(int itemId, CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IEventImportJobRepository>();
        var processor = scope.ServiceProvider.GetRequiredService<IEventImportImageProcessor>();

        try
        {
            var item = await repository.GetItemForUpdateAsync(itemId, cancellationToken);
            if (item == null)
            {
                return;
            }

            await processor.ProcessAsync(item, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            // Only a database error gets here (the processor records its own failures); the row stays Pending and the other rows go on.
            _logger.LogError(ex, "Could not process import image row {ItemId}", itemId);
        }
    }

    private DateTime UtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
}
