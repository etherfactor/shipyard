using EtherGizmos.Configuration;
using EtherGizmos.Messaging;
using EtherGizmos.Messaging.Configuration;
using EtherGizmos.Shipyard.Worker.Configuration;
using EtherGizmos.Shipyard.Worker.Services.HostedServices;
using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSerilog((services, logger) =>
    logger.ReadFrom.Configuration(services.GetRequiredService<IConfiguration>()));

//************************************************************
// Configuration

builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddRemappedEnvironmentVariables(
        (new(@"(?<=[^:_])_(?=[^_])"), "."),
        (new(@"(?<=[^_]):_(?=[^_])"), " "),
        (new(@"^ConnectionStrings:(?=[^_:])"), ""));

//************************************************************
// Services

builder.Services
    .AddOptions<SeleniumDriverOptions>()
    .Configure<IConfiguration>((opt, conf) =>
    {
        conf.GetSection("Selenium")
            .Bind(opt);
    });

builder.Services
    .AddMessaging((opt, conf) =>
    {
        opt.Listeners.AddQueue("tracking-poll-request", "tracking.poll.request");
        opt.Publishers.AddQueue("tracking-poll-request", "tracking.poll.request");

        opt.Listeners.AddQueue("tracking-poll-response", "tracking.poll.response");
        opt.Publishers.AddQueue("tracking-poll-response", "tracking.poll.response");
    })
    .UseRabbitMQ((opt, conf) =>
    {
        conf.GetSection("RabbitMQ")
            .Bind(opt);
    })
    .AddConsumersFromAssemblies(typeof(Program).Assembly);

builder.Services
    .AddTransient<SeleniumChromiumClient>()
    .AddTransient<IBrowserClient>(e =>
    {
        var client = e.GetRequiredService<SeleniumChromiumClient>();
        _ = client.StartAsync();

        return client;
    });

builder.Services.AddHostedService<QueueTrackingRequestBackgroundService>();

//************************************************************
// Pipeline

var app = builder.Build();

app.Run();
