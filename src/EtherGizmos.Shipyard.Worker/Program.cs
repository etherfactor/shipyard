using EtherGizmos.Common;
using EtherGizmos.Common.Configuration;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Services;
using EtherGizmos.Shipyard.Services.Api;
using EtherGizmos.Shipyard.Services.Carriers;
using EtherGizmos.Shipyard.Services.Handlers;
using EtherGizmos.Shipyard.Services.HostedServices;
using EtherGizmos.Shipyard.Services.WebDrivers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);

var otelEnabled = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT"))
    || !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT"))
    || !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_TRACES_ENDPOINT"))
    || !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_METRICS_ENDPOINT"));

builder.Logging.ClearProviders();

builder.Services
    .AddSerilog((services, logger) =>
    {
        logger.ReadFrom.Configuration(services.GetRequiredService<IConfiguration>());
        if (otelEnabled) logger.WriteTo.OpenTelemetry();
    },
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
    .AddOptions<ApiOptions>()
    .Bind(builder.Configuration.GetSection("Api"))
    .ValidateOnStart()
    .ValidateDataAnnotations();

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
    .WithRabbitMQ();

// Observability
if (otelEnabled)
{
    builder.Services
        .AddOpenTelemetry()
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddMessagingInstrumentation()
            .AddUnitOfWorkInstrumentation()
            .AddOtlpExporter())
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddMessagingInstrumentation()
            .AddUnitOfWorkInstrumentation()
            .AddOtlpExporter());
}

// Http
builder.Services.AddHttpClient("API")
    .ConfigureHttpClient((provider, client) =>
    {
        var apiOptions = provider.GetRequiredService<IOptionsMonitor<ApiOptions>>()
            .CurrentValue;

        client.BaseAddress = new(apiOptions.BaseUrl);
    })
    .AddHttpMessageHandler(provider => provider.GetRequiredService<ApiAuthenticationHandler>());

builder.Services.AddTransient<ApiAuthenticationHandler>();

builder.Services.AddSingleton<IArtifactSender, ArtifactSender>();

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

// Hosted Services
builder.Services.AddHostedService<QueueTrackingRequestBackgroundService>();

//************************************************************
// Pipeline

var app = builder.Build();

app.Run();
