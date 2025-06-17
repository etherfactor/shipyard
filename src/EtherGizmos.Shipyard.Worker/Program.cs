using EtherGizmos.Configuration;
using EtherGizmos.Messaging;
using EtherGizmos.Messaging.Configuration;
using EtherGizmos.Shipyard.Worker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddRemappedEnvironmentVariables(
        (new(@"(?<=[^:_])_(?=[^_])"), "."),
        (new(@"(?<=[^_]):_(?=[^_])"), " "),
        (new(@"^ConnectionStrings:(?=[^_:])"), ""));

builder.AddServiceDefaults();

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

builder.Services.AddHostedService<QueueTrackingRequestBackgroundService>();

var app = builder.Build();

app.Run();