using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EtherGizmos.Shipyard.Swagger;

public class OnlyJsonOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var response in operation.Responses)
        {
            var removeKeys = response.Value.Content.Keys.Where(e => e != "application/json" && e != "application/yaml");
            foreach (var removeKey in removeKeys)
            {
                response.Value.Content.Remove(removeKey);
            }

            if (operation.Tags[0].Name == "ImportExport")
            {
                response.Value.Content.Add("application/yaml", new() { });
                response.Value.Content.Add("application/json", new() { });
            }
        }
    }
}
