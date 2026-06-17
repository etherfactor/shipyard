using EtherGizmos.Common;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Configuration;
using EtherGizmos.Common.Services;
using EtherGizmos.Shipyard;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Events;
using EtherGizmos.Shipyard.Services;
using EtherGizmos.Shipyard.Services.Bootstrappers;
using EtherGizmos.Shipyard.Services.Export;
using EtherGizmos.Shipyard.Services.Formatters;
using EtherGizmos.Shipyard.Services.Health;
using EtherGizmos.Shipyard.Services.HostedServices;
using EtherGizmos.Shipyard.Services.Logging;
using EtherGizmos.Shipyard.Services.Middleware;
using EtherGizmos.Shipyard.Services.Pipeline.OAuth2;
using EtherGizmos.Shipyard.Services.Security;
using EtherGizmos.Shipyard.Services.Validators;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((services, logger) =>
    logger.ReadFrom.Configuration(services.GetRequiredService<IConfiguration>()));

//**********************************************************
// Configuration

builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddRemappedEnvironmentVariables(
        new(new(@"(?<=[^:_])_(?=[^_])"), "."),
        new(new(@"(?<=[^_]):_(?=[^_])"), " "),
        new(new(@"^ConnectionStrings:(?=[^_:])"), ""))
    .AddModularConfigurations(builder.Configuration);

builder.Services
    .AddOptions<WebUIOptions>()
    .Bind(builder.Configuration.GetSection("WebUI"))
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
    .AddOptions<LogIngestionOptions>()
    .Bind(builder.Configuration.GetSection("LogIngestion"))
    .ValidateOnStart()
    .ValidateDataAnnotations();

//**********************************************************
// Services

// General
builder.AddServiceDefaults();

builder.Services.AddConnectionResolver()
    .WithPostgreSql()
    .WithRabbitMQ()
    .WithSmtp();

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

builder.Services.AddSingleton<ICapabilityAuthorizer, CapabilityAuthorizer>();

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
    .AddDbContext<AuthorizationContext>((services, opt) =>
    {
        opt.UseLazyLoadingProxies();
        opt.EnableSensitiveDataLogging();

        var dbOptions = services
            .GetRequiredService<IOptionsMonitor<ConnectionReferenceOptions>>()
            .Get("Database");

        var connectionId = dbOptions.ConnectionId;

        var resolver = services.GetRequiredService<IConnectionResolver>();
        var connection = resolver.GetDatabaseConnection(connectionId);

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
        opt.Publishers.AddQueue("tracking-poll-request", "tracking.poll.request");
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
    .AddArtifactReader((opt, conf) =>
    {
        conf.GetSection("Artifacts")
            .Bind(opt);
    })
    .AddArtifactWriter((opt, conf) =>
    {
        conf.GetSection("Artifacts")
            .Bind(opt);
    });

// Models
builder.Services.AddModelValidators()
    .AddScoped<IModelValidator<Carrier>, CarrierValidator>();

// Controllers
builder.Services
    .AddRouting(opt =>
    {
        opt.LowercaseUrls = true;
    })
    .AddControllersWithViews()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services
    .AddMvc(opt =>
    {
        opt.InputFormatters.Add(new YamlInputFormatter());
        opt.OutputFormatters.Add(new YamlOutputFormatter());
        opt.FormatterMappings.SetMediaTypeMappingForFormat("yaml", "application/yaml");
    });

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

builder.Services.AddSingleton<ISourceLoggerFactory, SourceLoggerFactory>();

builder.Services.AddHostedService<BootstrapSeeder>();
builder.Services.AddSingleton<IBootstrapper, AppBootstrapper>();
builder.Services.AddSingleton<IBootstrapper, NotificationBootstrapper>();
builder.Services.AddSingleton<IBootstrapper, OAuth2Bootstrapper>();

// Export & Import
builder.Services.AddSingleton<IExportDocumentMigrator, ExportDocumentMigrator>();
builder.Services.AddScoped<IExportDocumentImporterRegistry, ExportDocumentImporterRegistry>();
builder.Services.AddScoped<IExportDocumentImporter, CarrierImporter>();

// Notifications
builder.Services.AddNotifications(
    builder.Configuration["Database:ConnectionId"]!,
    builder.Configuration["MessageBroker:ConnectionId"]!,
    opt =>
    {
        opt.AddWebhookChannel();

        var emailConnectionId = builder.Configuration["Email:ConnectionId"];
        if (emailConnectionId is not null)
            opt.AddEmailChannel(emailConnectionId);

        opt.AddShipyardExtractors();

        opt.AddNotification<PackageDeliveredEvent, PackageDeliveredRouter>(
            "package.delivered",
            evt =>
            {
                evt.HasDisplayName("Package Delivered");

                evt.Supports<PackageDeliveredEvent, EmailChannel, PackageDeliveredEmailFormatter>();

                evt.Supports<PackageDeliveredEvent, WebhookChannel, PackageDeliveredWebhookFormatter>();
                evt.SupportsDigest<PackageDeliveredEvent, WebhookChannel, PackageDeliveredDigestWebhookFormatter>();
            });
    });

// Health
builder.Services
    .AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(
        name: "database",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["database"]);

// Documentation
builder.Services
    .AddSwaggerGen(opt =>
    {
        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token",
        });

        opt.AddSecurityRequirement(document => new OpenApiSecurityRequirement()
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = [],
        });
    });

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
