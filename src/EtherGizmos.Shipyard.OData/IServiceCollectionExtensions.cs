using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard;
using EtherGizmos.Shipyard.Services;
using EtherGizmos.Shipyard.Services.Filters;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;
using ODataOptions = EtherGizmos.Shipyard.Configuration.ODataOptions;

namespace EtherGizmos.Shipyard;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddOData(
        this IServiceCollection @this,
        Action<ODataOptions, IConfiguration> configureOptions)
    {
        @this.AddOptions<ODataOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        @this.AddOptions<ODataApiVersioningOptions>()
            .Configure<IOptions<ODataOptions>>((opt, @ref) =>
            {
                foreach (var route in @ref.Value.VersionedRoutePrefixes)
                {
                    opt.AddRouteComponents(route);
                }
            });

        @this.AddOptions<ApiVersioningOptions>()
            .Configure<IOptions<ODataOptions>>((opt, @ref) =>
            {
                opt.DefaultApiVersion = @ref.Value.DefaultApiVersion;
            });

        @this.AddOptions<ApiBehaviorOptions>()
            .Configure(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;
            });

        @this
            .AddControllers(opt =>
            {
                opt.Filters.Add<ModelStateActionFilter>();
            })
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .AddOData();

        @this
            .AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc()
            .AddApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'VVV";
                opt.SubstituteApiVersionInUrl = true;
            })
            .AddOData(opt =>
            {

            })
            .AddODataApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'VVV";
                opt.SubstituteApiVersionInUrl = true;
            });

        @this.AddTransient(typeof(IModelValidator<>), typeof(ValidationModelValidator<>));

        var fakeOptions = new ODataOptions();
        configureOptions(fakeOptions, new ConfigurationManager());

        @this.AddAutoMapper(fakeOptions.ModelAssemblies);

        @this.AddEndpointsApiExplorer();
        @this.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        @this
            .AddSwaggerGen(opt => { })
            .AddSwaggerExamplesFromAssemblies([.. fakeOptions.ModelAssemblies])
            .AddOptions<SwaggerGenOptions>()
            .Configure<IOptions<ODataOptions>>((opt, @ref) =>
            {
                var file = @ref.Value.ExecutingAssembly.GetName().Name + ".xml";
                var path = Path.Combine(AppContext.BaseDirectory, file);
                if (File.Exists(path))
                {
                    opt.IncludeXmlComments(path);
                }

                //Add a custom operation filter which sets default values
                opt.OperationFilter<SwaggerDefaultValues>();

                opt.CustomSchemaIds(x => x.Name.Replace("DTO", ""));

                opt.OperationFilter<OnlyJsonOperationFilter>();
                opt.ExampleFilters();
                opt.OperationFilter<ResponseSetFilter>();
            });

        return @this;
    }
}
