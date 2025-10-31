using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NCrontab;

namespace EtherGizmos.Shipyard.Worker.Services.HostedServices;

public abstract class PeriodicBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    private readonly CrontabSchedule _schedule;

    public PeriodicBackgroundService(
        string cronExpression,
        IServiceProvider serviceProvider,
        ILogger? logger = null)
    {
        _serviceProvider = serviceProvider;
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

                using var scope = _serviceProvider.CreateScope();
                await ExecuteIterationAsync(scope.ServiceProvider, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Encountered an error during the loop for {ServiceName}", GetType().Name);
            }
        }
    }

    protected abstract Task ExecuteIterationAsync(IServiceProvider provider, CancellationToken stoppingToken);
}
