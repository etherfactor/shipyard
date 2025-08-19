using EtherGizmos.Common;
using EtherGizmos.Common.Configuration;
using EtherGizmos.Shipyard;
using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Models;
using EtherGizmos.Shipyard.Services;
using EtherGizmos.Shipyard.Worker.Configuration;
using EtherGizmos.Shipyard.Worker.Services.Carriers;
using EtherGizmos.Shipyard.Worker.Services.HostedServices;
using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

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

builder.Services
    .AddOptions<NotificationOptions>()
    .Configure<IConfiguration>((opt, conf) =>
    {
        conf.GetSection("Notifications")
            .Bind(opt);
    })
    .ValidateOnStart()
    .ValidateDataAnnotations();

builder.Services
    .AddOptions<SeleniumDriverOptions>()
    .Configure<IConfiguration>((opt, conf) =>
    {
        conf.GetSection("Selenium")
            .Bind(opt);
    })
    .ValidateDataAnnotations()
    .ValidateOnStart();

//************************************************************
// Services

// General
builder.AddServiceDefaults();

builder.Services.AddServiceConnections();

// Database
builder.Services
    .AddDatabase()
    .AddUnitOfWork(opt =>
    {
        opt.BindDbContext<ApplicationContext>();
    });

// Messaging
builder.Services
    .AddMessaging((opt, conf) =>
    {
        opt.Listeners.AddQueue("tracking-poll-request", "tracking.poll.request");
        opt.Publishers.AddQueue("tracking-poll-request", "tracking.poll.request");

        opt.Listeners.AddQueue("tracking-poll-response", "tracking.poll.response");
        opt.Publishers.AddQueue("tracking-poll-response", "tracking.poll.response");

        opt.Listeners.AddTopic("notification-package-outfordelivery", "notification.package.outfordelivery", subscription: "email");
        opt.Publishers.AddTopic("notification-package-outfordelivery", "notification.package.outfordelivery");

        opt.Listeners.AddTopic("notification-package-delivered", "notification.package.delivered", subscription: "email");
        opt.Publishers.AddTopic("notification-package-delivered", "notification.package.delivered");
    })
    .UseRabbitMQ((opt, conf) =>
    {
        conf.GetSection("RabbitMq")
            .Bind(opt);
    })
    .AddConsumersFromAssemblies(typeof(Program).Assembly);

// Tracking
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

// Notifications
builder.Services.AddNotifications(typeof(Program).Assembly, typeof(NotificationEvent).Assembly);

// Hosted Services
builder.Services.AddHostedService<QueueTrackingRequestBackgroundService>();

//************************************************************
// Pipeline

var app = builder.Build();

app.Run();
