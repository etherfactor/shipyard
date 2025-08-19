using EtherGizmos.Common;
using EtherGizmos.Common.Configuration;
using EtherGizmos.Common.Services;
using EtherGizmos.Shipyard;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Api.Services.HostedServices;
using EtherGizmos.Shipyard.Api.Services.Middleware;
using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

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

//**********************************************************
// Services

// General
builder.AddServiceDefaults();

builder.Services.AddServiceConnections();

// Security
builder.UseOAuth2()
    .AsAuthorizationServer<AuthorizationContext>(opt =>
    {
        opt.Cookie.LoginUrl = "/account/login";
        opt.Cookie.LogoutUrl = "/account/logout";
    });

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
    });

// Models
builder.Services.AddModelValidators();

// Controllers
builder.Services.AddControllers();

builder.Services
    .AddOData((opt, conf) =>
    {
        opt.DefaultApiVersion = ApiVersions.V0_1;
        opt.VersionedRoutePrefixes = ["api/v{version:apiVersion}"];
        opt.ExecutingAssembly = typeof(Program).Assembly;
        opt.ModelAssemblies = [typeof(Package).Assembly];
    });

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddHostedService<OAuth2Seeder>();

//**********************************************************
// Pipeline

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
