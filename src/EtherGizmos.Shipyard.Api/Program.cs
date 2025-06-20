using EtherGizmos.Configuration;
using EtherGizmos.Shipyard.Api.Services.Middleware;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Configuration;
using EtherGizmos.Shipyard.Database.Services;
using EtherGizmos.Shipyard.Models;
using EtherGizmos.Shipyard.Models.Api.Errors;
using EtherGizmos.Shipyard.Models.Database;
using EtherGizmos.Shipyard.OData;
using EtherGizmos.Shipyard.Utilities;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.OData;

var builder = WebApplication.CreateBuilder(args);

//**********************************************************
// Configuration

builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddRemappedEnvironmentVariables(
        (new(@"(?<=[^:_])_(?=[^_])"), "."),
        (new(@"(?<=[^_]):_(?=[^_])"), " "),
        (new(@"^ConnectionStrings:(?=[^_:])"), ""));

builder.Services
    .AddOptions<PostgreSqlOptions>()
    .Configure<IConfiguration>((opt, conf) =>
    {
        conf.GetSection("PostgreSql")
            .Bind(opt);
    })
    .ValidateDataAnnotations()
    .ValidateOnStart();

//**********************************************************
// Add Services

// General
builder.AddServiceDefaults();

// Database
builder.Services
    .AddDatabase()
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

//**********************************************************
// Add Middleware

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
