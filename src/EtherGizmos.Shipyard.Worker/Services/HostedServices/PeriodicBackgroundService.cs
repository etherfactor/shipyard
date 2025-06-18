using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NCrontab;

namespace EtherGizmos.Shipyard.Worker.Services.HostedServices;

public abstract class PeriodicBackgroundService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly CrontabSchedule _schedule;

    public PeriodicBackgroundService(
        string cronExpression,
        ILogger? logger = null)
    {
        _logger = logger ?? NullLogger.Instance;

        if (cronExpression.Split(' ').Length >= 5)
        {
            _schedule = CrontabSchedule.Parse(cronExpression, new() { IncludingSeconds = true });
        }
        else
        {
            _schedule = CrontabSchedule.Parse(cronExpression);
        }
    }

    protected sealed override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var next = new DateTimeOffset(_schedule.GetNextOccurrence(DateTime.UtcNow), TimeSpan.Zero);
                var wait = next - DateTimeOffset.UtcNow;

                if (wait.Ticks > 0)
                {
                    await Task.Delay(wait, stoppingToken);
                }

                await ExecuteIterationAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Encountered an error during the loop for {ServiceName}", GetType().Name);
            }
        }
    }

    protected abstract Task ExecuteIterationAsync(CancellationToken stoppingToken);
}
