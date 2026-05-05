using EtherGizmos.Common;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Configuration;
using EtherGizmos.Common.Services;
using EtherGizmos.Shipyard;
using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Models;
using EtherGizmos.Shipyard.Services;
using EtherGizmos.Shipyard.Worker.Configuration;
using EtherGizmos.Shipyard.Worker.Services.Carriers;
using EtherGizmos.Shipyard.Worker.Services.HostedServices;
using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();

builder.Services.AddSerilog((services, logger) =>
    logger.ReadFrom.Configuration(services.GetRequiredService<IConfiguration>()),
    writeToProviders: true);

builder.Services.AddTeeStreamLogger();

//************************************************************
// Configuration

builder.Configuration
    .AddJsonFile($"appsettings.{builder.Environment}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddRemappedEnvironmentVariables(
        new(new(@"(?<=[^:_])_(?=[^_])"), "."),
        new(new(@"(?<=[^_]):_(?=[^_])"), " "),
        new(new(@"^ConnectionStrings:(?=[^_:])"), ""))
    .AddModularConfigurations(builder.Configuration);

builder.Services
    .AddOptions<ConnectionReferenceOptions>("Database")
    .Bind(builder.Configuration.GetSection("Database"))
    .ValidateOnStart()
    .ValidateDataAnnotations();

builder.Services
    .AddOptions<ConnectionReferenceOptions>("MessageBroker")
    .Bind(builder.Configuration.GetSection("MessageBroker"))
    .ValidateOnStart()
    .ValidateDataAnnotations();

builder.Services
    .AddOptions<NotificationOptions>()
    .Bind(builder.Configuration.GetSection("Notifications"))
    .ValidateOnStart()
    .ValidateDataAnnotations();

builder.Services
    .AddOptions<SeleniumDriverOptions>()
    .Bind(builder.Configuration.GetSection("Selenium"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<WorkerOptions>()
    .Bind(builder.Configuration.GetSection("Worker"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

//************************************************************
// Services

// General
builder.AddServiceDefaults();

builder.Services.AddConnectionResolver()
    .WithPostgreSql()
    .WithRabbitMQ();

// Database
builder.Services
    .AddDbContext<ApplicationContext>((services, opt) =>
    {
        opt.UseLazyLoadingProxies();
        opt.EnableSensitiveDataLogging();

        var dbOptions = services
            .GetRequiredService<IOptionsMonitor<ConnectionReferenceOptions>>()
            .Get("Database");

        var connectionId = dbOptions.ConnectionId;

        var resolver = services.GetRequiredService<IConnectionResolver>();

        opt.UseConnection(services, connectionId);
    })
    .AddUnitOfWork(opt =>
    {
        opt.BindDbContext<ApplicationContext>();
        opt.BindDbContext<ArtifactContext>();
    });

builder.Services.AddScoped<IFilterContext, FilterContext>();

builder.Services
    .AddMigrations("Application", typeof(ApplicationContext).Assembly)
    .UseConnection(builder.Configuration["Database:ConnectionId"] ?? "!Unknown");

builder.Services.AddHttpContextAccessor();
builder.Services.TryAddSingleton<IUserContext, UserContext>();

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
    .UseConnection(builder.Configuration["MessageBroker:ConnectionId"] ?? "!Unknown")
    .AddConsumersFromAssemblies(typeof(Program).Assembly);

builder.Services
    .AddOptions<JsonSerializerOptions>("Messaging")
    .Configure(opt =>
    {
        opt.Converters.Add(new ArtifactUriConverter());
    });

// Storage
builder.Services
    .AddArtifactWriter((opt, conf) =>
    {
        conf.GetSection("Artifacts")
            .Bind(opt);
    });

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
