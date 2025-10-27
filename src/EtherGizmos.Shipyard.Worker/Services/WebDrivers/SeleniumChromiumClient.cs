using EtherGizmos.Shipyard.Worker.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Collections.Concurrent;

namespace EtherGizmos.Shipyard.Worker.Services.WebDrivers;

internal class SeleniumChromiumClient : IBrowserClient, IDisposable
{
    private readonly ILogger _logger;
    private readonly SeleniumDriverOptions _options;

    private RemoteWebDriver? _driver;
    private SemaphoreSlim? _semaphore;

    private bool _disposed;

    public SeleniumChromiumClient(
        ILogger<SeleniumChromiumClient> logger,
        IOptions<SeleniumDriverOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task StartAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Waiting for Selenium container {ContainerUri}", _options.ConnectionString);

        _semaphore = SeleniumCoordinator.ForUrl(_options.ConnectionString);
        await _semaphore.WaitAsync(cancellationToken);

        var gridUri = new Uri(_options.ConnectionString);

        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--window-size=1920,1080");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");

        options.AddExcludedArgument("enable-automation");
        options.AddAdditionalOption("useAutomationExtension", false);

        _driver = new RemoteWebDriver(gridUri, options.ToCapabilities());

        _logger.LogInformation("Connected to Selenium container {ContainerUri}", _options.ConnectionString);
    }

    public Task StopAsync(
        CancellationToken cancellationToken = default)
    {
        _driver?.Dispose();
        _semaphore?.Release();

        _logger.LogInformation("Disconnected from Selenium container {ContainerUri}", _options.ConnectionString);

        return Task.CompletedTask;
    }

    public async Task NavigateAsync(
        string requestUrl,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        while (_driver is null)
        {
            await Task.Delay(100, cancellationToken);
        }

        _logger.LogInformation("Navigating to url {RequestUri}", requestUrl);

        await _driver.Navigate().GoToUrlAsync(requestUrl);

        _logger.LogInformation("Navigated to url {RequestUri}", requestUrl);

        var wait = new WebDriverWait(_driver, timeout ?? TimeSpan.FromSeconds(30));
        wait.Until(d =>
            ((IJavaScriptExecutor)d)
              .ExecuteScript("return document.readyState") as string == "complete");
    }

    public async Task WaitForElementAsync(
        string selector,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        while (_driver is null)
        {
            await Task.Delay(100, cancellationToken);
        }

        _logger.LogInformation("Waiting until element {CssSelector} is loaded", selector);

        var wait = new WebDriverWait(_driver, timeout ?? TimeSpan.FromSeconds(30));
        wait.Until(d => d.FindElement(By.CssSelector(selector)).Displayed);

        _logger.LogInformation("Element {CssSelector} is loaded", selector);
    }

    public async Task ClickElementAsync(
        string selector,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        while (_driver is null)
        {
            await Task.Delay(100, cancellationToken);
        }

        _logger.LogInformation("Clicking element {CssSelector}", selector);

        var element = _driver.FindElement(By.CssSelector(selector));
        try
        {
            element.Click();
        }
        catch (ElementClickInterceptedException)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", element);
        }

        _logger.LogInformation("Clicked element {CssSelector}", selector);
    }

    public async Task<string> GetHtmlAsync(
        CancellationToken cancellationToken = default)
    {
        while (_driver is null)
        {
            await Task.Delay(100, cancellationToken);
        }

        return _driver.PageSource;
    }

    public async Task SendAsync(
        string selector,
        string content,
        CancellationToken cancellationToken = default)
    {
        while (_driver is null)
        {
            await Task.Delay(100, cancellationToken);
        }

        _logger.LogInformation("Sending element {CssSelector} content {Content}", selector, content);

        _driver.FindElement(By.CssSelector(selector)).SendKeys(content);

        _logger.LogInformation("Sent element {CssSelector} content {Content}", selector, content);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                StopAsync().Wait();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private static class SeleniumCoordinator
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = [];

        public static SemaphoreSlim ForUrl(
            string uri)
        {
            return _semaphores.GetOrAdd(uri, _ => new SemaphoreSlim(1, 1));
        }
    }
}
