namespace EtherGizmos.Shipyard.Worker.Services.WebDrivers;

public interface IBrowserClient : IDisposable
{
    Task ClickElementAsync(string selector, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    Task<string> GetHtmlAsync(CancellationToken cancellationToken = default);

    Task NavigateAsync(string requestUrl, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);

    Task WaitForElementAsync(string selector, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
}
