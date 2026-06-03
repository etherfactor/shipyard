using EtherGizmos.Shipyard.Extensions;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Nodes;

namespace EtherGizmos.Shipyard.Swagger;

public class ODataContextOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.DeclaringType is null
            || !context.MethodInfo.DeclaringType.IsAssignableTo(typeof(ODataController)))
            return;

        operation.Responses ??= [];

        foreach (var response in operation.Responses.Values)
        {
            if (response.Content is null)
                continue;

            //Only want to consider removing content types if this is an OData endpoint, otherwise we break things
            if (!response.Content.Keys.Any(e => e.Contains("odata", StringComparison.OrdinalIgnoreCase)))
                continue;

            foreach (var (_, value) in response.Content)
            {
                if (value.Example is null)
                    continue;

                var example = value.Example;
                if (example is null || example.IsODataError())
                    continue;

                var oldObj = example.AsObject();
                var newObj = new JsonObject()
                {
                    ["@odata.context"] = "https://.../$metadata/#...",
                };

                var keys = oldObj.Select(e => e.Key).ToList();
                foreach (var key in keys)
                {
                    oldObj.Remove(key, out var node);
                    newObj.Add(key, node);
                }

                value.Example = newObj;
            }
        }
    }
}
