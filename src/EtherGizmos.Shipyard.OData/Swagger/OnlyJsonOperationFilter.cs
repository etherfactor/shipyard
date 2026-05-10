using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EtherGizmos.Shipyard.Swagger;

public class OnlyJsonOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
        {
            operation.Responses ??= [];

            var key = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
            var response = operation.Responses[key];

            if (response.Content is null)
                continue;

            //Only want to consider removing content types if this is an OData endpoint, otherwise we break things
            if (!response.Content.Keys.Any(e => e.Contains("odata", StringComparison.OrdinalIgnoreCase)))
                continue;

            foreach (var contentType in response.Content.Keys)
            {
                switch (contentType.ToLower())
                {
                    case "application/xml":
                    case "text/plain":
                    case "application/octet-stream":
                    case string s when s.Contains("odata.streaming"):
                        response.Content.Remove(contentType);
                        break;
                }
            }

            if (operation.Tags[0].Name == "ImportExport")
            {
                response.Value.Content.TryAdd("application/yaml", new() { });
                response.Value.Content.TryAdd("application/json", new() { });
            }
        }
    }
}
