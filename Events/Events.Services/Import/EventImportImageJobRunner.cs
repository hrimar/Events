using Events.Data.Repositories.Interfaces;
using Events.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Events.Services.Import;

// Copies the images of one Excel import inside the Confirm request, and again for "Retry remaining" on the report
// (GS-REQ-009, 9.1, 9.3). No background worker: the owner imports at most about 50 rows at once (owner decision,
// 23.11.2026, instead of 9.3.4). Rows run four at a time, each in its own DI scope (DbContext is not thread-safe)
// and saved as soon as it is done. The run stops after TimeBudget, well below the 230 s request limit of Azure App
// Service; rows not reached stay Pending, are shown as "Not processed" and can be retried or uploaded by hand.
public sealed class EventImportImageJobRunner
{
    public const int MaxParallelDownloads = 4;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<EventImportImageJobRunner> _logger;

    public EventImportImageJobRunner(IServiceScopeFactory scopeFactory, TimeProvider timeProvider, ILogger<EventImportImageJobRunner> logger)
    {
        _scopeFactory = scopeFactory;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    // 50 rows normally take 15-40 s; even when every address times out (10 s each) four at a time reach ~125 s.
    public TimeSpan TimeBudget { get; init; } = TimeSpan.FromSeconds(150);

    // Processes the job's Pending rows; returns how many are still Pending afterwards (0 when all were reached).
    public async Task<int> RunAsync(int jobId, CancellationToken cancellationToken = default)
    {
        List<int> itemIds;
        await using (var scope = _scopeFactory.CreateAsyncScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IEventImportJobRepository>();
            if (!await repository.StartAsync(jobId, UtcNow(), cancellationToken))
            {
                _logger.LogWarning("Import image job {JobId} not found", jobId);
                return 0;
            }

            itemIds = await repository.GetPendingItemIdsAsync(jobId, cancellationToken);
        }

        _logger.LogInformation("Import image job {JobId}: {Count} row(s) to process", jobId, itemIds.Count);

        using var budget = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        budget.CancelAfter(TimeBudget);
        try
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = MaxParallelDownloads, CancellationToken = budget.Token };
            await Parallel.ForEachAsync(itemIds, options, async (itemId, token) => await ProcessItemAsync(itemId, token));
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Import image job {JobId} stopped after {Seconds} s; the remaining rows stay Pending", jobId, TimeBudget.TotalSeconds);
        }

        await using (var scope = _scopeFactory.CreateAsyncScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IEventImportJobRepository>();
            await repository.CompleteAsync(jobId, UtcNow(), cancellationToken);
            var stillPending = (await repository.GetPendingItemIdsAsync(jobId, cancellationToken)).Count;

            _logger.LogInformation("Import image job {JobId} done; {Count} row(s) not processed", jobId, stillPending);
            return stillPending;
        }
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
