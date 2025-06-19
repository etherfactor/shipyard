using Asp.Versioning;
using EtherGizmos.Shipyard.OData.Configuration;
using EtherGizmos.Shipyard.OData.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EtherGizmos.Shipyard.OData;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddOData(
        this IServiceCollection @this,
        Action<ODataOptions, IConfiguration> configureOptions)
    {
        var useOptions = new ODataOptions();

        @this.AddOptions<ODataOptions>()
            .Configure(configureOptions)
            .PostConfigure(options =>
            {
                useOptions = options;
            })
            .ValidateDataAnnotations()
            .ValidateOnStart();

        @this.AddOptions<ApiBehaviorOptions>()
            .Configure(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;
            });

        @this
            .AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.DefaultApiVersion = useOptions.DefaultApiVersion;
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
                foreach (var route in useOptions.VersionedRoutePrefixes)
                {
                    opt.AddRouteComponents(route);
                }
            })
            .AddODataApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'VVV";
                opt.SubstituteApiVersionInUrl = true;
            });

        @this.AddEndpointsApiExplorer();
        @this.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        @this
            .AddSwaggerGen(opt =>
            {
                var file = useOptions.ExecutingAssembly.GetName().Name + ".xml";
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
            })
            .AddSwaggerExamplesFromAssemblies([.. useOptions.ModelAssemblies]);

        return @this;
    }
}
