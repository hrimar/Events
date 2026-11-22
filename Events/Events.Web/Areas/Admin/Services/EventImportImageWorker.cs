using Events.Data.Repositories.Interfaces;
using Events.Services.Import;

namespace Events.Web.Areas.Admin.Services;

// Background worker that copies Excel import images, so a 200-row import does not block the admin UI
// (GS-REQ-009, 9.3.4). It polls the database: the oldest Queued or Running job is run, otherwise it waits a few
// seconds. All state lives in the database, so a restart or deploy simply continues the unfinished job.
// Every error is caught: in .NET 8 an exception escaping a BackgroundService stops the whole web app.
public sealed class EventImportImageWorker : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);

    private readonly EventImportImageJobRunner _runner;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EventImportImageWorker> _logger;

    public EventImportImageWorker(EventImportImageJobRunner runner, IServiceScopeFactory scopeFactory, ILogger<EventImportImageWorker> logger)
    {
        _runner = runner;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var jobId = await GetNextJobIdAsync(stoppingToken);
                if (jobId != null)
                {
                    await _runner.RunAsync(jobId.Value, stoppingToken);
                    continue;
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Import image worker error; retrying in {Seconds} s", PollInterval.TotalSeconds);
            }

            try
            {
                await Task.Delay(PollInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    private async Task<int?> GetNextJobIdAsync(CancellationToken stoppingToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IEventImportJobRepository>();
        var jobIds = await repository.GetUnfinishedJobIdsAsync(stoppingToken);
        return jobIds.Count > 0 ? jobIds[0] : null;
    }
}
