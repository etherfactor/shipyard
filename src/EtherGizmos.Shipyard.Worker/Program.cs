using EtherGizmos.Common;
using EtherGizmos.Common.Configuration;
using EtherGizmos.Common.Messaging;
using EtherGizmos.Common.Messaging.Configuration;
using EtherGizmos.Common.Utilities;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Configuration;
using EtherGizmos.Shipyard.Database.Services;
using EtherGizmos.Shipyard.Worker.Configuration;
using EtherGizmos.Shipyard.Worker.Services.Carriers;
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

builder.Services
    .AddOptions<DatabaseReferenceOptions>()
    .Configure<IConfiguration>((opt, conf) =>
    {
        conf.GetSection("Database")
            .Bind(opt);
    })
    .ValidateOnStart()
    .ValidateDataAnnotations();

//************************************************************
// Services

builder.Services.AddServiceConnections();

builder.Services
    .AddOptions<SeleniumDriverOptions>()
    .Configure<IConfiguration>((opt, conf) =>
    {
        conf.GetSection("Selenium")
            .Bind(opt);
    })
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddDatabase()
    .AddUnitOfWork(opt =>
    {
        opt.BindDbContext<ApplicationContext>();
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
        conf.GetSection("RabbitMq")
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

builder.Services
    .AddSingleton<ITrackingProviderFactory, TrackingProviderFactory>()
    .AddTransient<IRegexClassifier, RegexClassifier>();

builder.Services.AddHostedService<QueueTrackingRequestBackgroundService>();

//************************************************************
// Pipeline

var app = builder.Build();

app.Run();
