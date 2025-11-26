using EtherGizmos.Common;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Configuration;
using EtherGizmos.Common.Services;
using EtherGizmos.Shipyard;
using EtherGizmos.Shipyard.Api.Abstractions;
using EtherGizmos.Shipyard.Api.Configuration;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Api.Services.Health;
using EtherGizmos.Shipyard.Api.Services.HostedServices;
using EtherGizmos.Shipyard.Api.Services.Logging;
using EtherGizmos.Shipyard.Api.Services.Middleware;
using EtherGizmos.Shipyard.Api.Services.Pipeline.OAuth2;
using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Services;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((services, logger) =>
    logger.ReadFrom.Configuration(services.GetRequiredService<IConfiguration>()));

//**********************************************************
// Configuration

builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddRemappedEnvironmentVariables(
        (new(@"(?<=[^:_])_(?=[^_])"), "."),
        (new(@"(?<=[^_]):_(?=[^_])"), " "),
        (new(@"^ConnectionStrings:(?=[^_:])"), ""))
    .AddExpandedConnections(builder.Configuration);

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
    .AddOptions<LogIngestionOptions>()
    .Configure<IConfiguration>((opt, config) =>
    {
        config.GetSection("LogIngestion")
            .Bind(opt);
    })
    .ValidateDataAnnotations()
    .ValidateOnStart();

//**********************************************************
// Services

// General
builder.AddServiceDefaults();

builder.Services.AddServiceConnections();

// Security
builder.UseOAuth2()
    .AsAuthorizationServer<AuthorizationContext>(opt =>
    {
        builder.Configuration
            .GetSection("Security")
            .Bind(opt);

        opt.ScanAssemblies =
        [
            typeof(ApplicationContext).Assembly,
            typeof(User).Assembly,
        ];

        opt.Cookie.LoginUrl = "/account/login";
        opt.Cookie.LogoutUrl = "/account/logout";
    });

builder.Services.AddScoped<IClaimsPipelineStep<OAuth2PrincipalContext>, OAuth2SetUserCapabilitiesStep>();
builder.Services.AddScoped<IClaimsPipelineStep<OAuth2PrincipalContext>, OAuth2SetUsernameStep>();
builder.Services.AddScoped<IClaimsPipelineStep<OAuth2PrincipalContext>, OAuth2ApplyDestinationsStep>();

// Database
builder.Services
    .AddDatabase()
    .AddDbContext<AuthorizationContext>((services, opt) =>
    {
        opt.UseLazyLoadingProxies();
        opt.EnableSensitiveDataLogging();

        var dbOptions = services.GetRequiredService<IOptions<DatabaseReferenceOptions>>()
            .Value;

        var connectionId = dbOptions.ConnectionId;

        var resolver = services.GetRequiredService<IConnectionResolver>();
        var connection = resolver.GetDatabaseConnection(connectionId);

        connection.Match(
            _ => throw new InvalidOperationException($"The connection {connectionId} is not a valid database connection."),
            postgreSql =>
            {
                return opt.UseNpgsql(
                    postgreSql.ConnectionString,
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            }
        );
    })
    .AddUnitOfWork(opt =>
    {
        opt.BindDbContext<ApplicationContext>();
        opt.BindDbContext<ArtifactContext>();
    });

// Messaging
builder.Services
    .AddMessaging((opt, conf) =>
    {
        opt.Publishers.AddQueue("tracking-poll-request", "tracking.poll.request");
    })
    .UseRabbitMQ((opt, conf) =>
    {
        conf.GetSection("RabbitMq")
            .Bind(opt);
    })
    .AddConsumersFromAssemblies(typeof(Program).Assembly);

builder.Services
    .AddOptions<JsonSerializerOptions>("Messaging")
    .Configure(opt =>
    {
        opt.Converters.Add(new ArtifactUriConverter());
    });

// Storage
builder.Services
    .AddArtifactReader((opt, conf) =>
    {
        conf.GetSection("Artifacts")
            .Bind(opt);
    });

// Models
builder.Services.AddModelValidators();

// Controllers
builder.Services
    .AddRouting(opt =>
    {
        opt.LowercaseUrls = true;
    })
    .AddControllersWithViews();

builder.Services
    .AddOData((opt, conf) =>
    {
        opt.DefaultApiVersion = ApiVersions.V0_1;
        opt.VersionedRoutePrefixes = ["api/v{version:apiVersion}"];
        opt.ExecutingAssembly = typeof(Program).Assembly;
        opt.ModelAssemblies = [typeof(Package).Assembly];
    });

builder.Services
    .AddHttpLogging(opt =>
    {
        opt.LoggingFields = HttpLoggingFields.All | HttpLoggingFields.RequestQuery;

        opt.RequestHeaders.Add("Origin");
        opt.RequestHeaders.Add("Priority");
        opt.RequestHeaders.Add("Referer");
        opt.RequestHeaders.Add("Sec-CH-UA");
        opt.RequestHeaders.Add("Sec-CH-UA-Mobile");
        opt.RequestHeaders.Add("Sec-CU-UA-Platform");
        opt.RequestHeaders.Add("Sec-Fetch-Site");
        opt.RequestHeaders.Add("Sec-Fetch-Mode");
        opt.RequestHeaders.Add("Sec-Fetch-Dest");

        opt.ResponseHeaders.Add("Access-Control-Allow-Headers");
        opt.ResponseHeaders.Add("Access-Control-Allow-Methods");
        opt.ResponseHeaders.Add("Access-Control-Allow-Origin");
        opt.ResponseHeaders.Add("OData-Version");

        opt.MediaTypeOptions.AddText("application/json");

        opt.RequestBodyLogLimit = 8192;
        opt.ResponseBodyLogLimit = 1024;
    });

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<ISourceLoggerFactory, SourceLoggerFactory>();

builder.Services.AddHostedService<InitialConfigSeeder>();
builder.Services.AddHostedService<OAuth2Seeder>();

// Health
builder.Services
    .AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(
        name: "database",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["database"]);

// Rendering
builder.Services
    .AddJsEngineSwitcher(opt => opt.DefaultEngineName = V8JsEngine.EngineName)
    .AddV8();

builder.Services
    .AddWebOptimizer(opt =>
    {
        opt.CompileScssFiles();
        opt.MinifyCssFiles();
        opt.MinifyJsFiles();
    });

//**********************************************************
// Pipeline

var app = builder.Build();

app.UseForwardedHeaders(
    new()
    {
        ForwardedHeaders =
            ForwardedHeaders.XForwardedFor |
            ForwardedHeaders.XForwardedProto |
            ForwardedHeaders.XForwardedHost
    });

app.UseHttpsRedirection();

app.UseWebOptimizer();
app.UseStaticFiles();

app.UseRouting();

app.UseHttpLogging();

app.
    UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var pathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

            if (pathFeature?.Error is ReturnErrorException ex)
            {
                await context.Response.WriteErrorAsync(ex.Error);
            }
        });
    });

app.UseMiddleware<ReturnErrorExceptionMiddleware>();

app
    .UseCors(opt =>
    {
        opt.AllowAnyOrigin();
        opt.AllowAnyMethod();
        opt.AllowAnyHeader();
    });

app
    .Use(async (context, next) =>
    {
        context.Request.EnableBuffering();
        await next();
    });

app.UseSwagger()
    .UseSwaggerUI(opt =>
    {
        var descriptions = app.DescribeApiVersions();

        // build a swagger endpoint for each discovered API version
        foreach (var description in descriptions)
        {
            var url = $"{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            opt.SwaggerEndpoint(url, name);
        }
    });

app.UseODataRouteDebug();

app
    .MapHealthChecks("/health", new HealthCheckOptions()
    {
        ResponseWriter = HealthExtensions.WriteResponse
    });

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
