using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Json.Nodes;

namespace EtherGizmos.Shipyard.Swagger;

public class ResponseSetFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.GetCustomAttribute<ProducesResponseSetAttribute>() is null)
            return;

        foreach (var response in operation.Responses)
        {
            foreach (var content in response.Value.Content)
            {
                var anyExample = content.Value.Example;
                if (anyExample is null)
                    continue;

                if (anyExample is OpenApiString stringExample)
                {
                    var deserialized = JsonNode.Parse(stringExample.Value);
                    if (deserialized is not null)
                    {
                        var newArray = new JsonArray()
                        {
                            deserialized,
                        };

                        var newObject = new JsonObject()
                        {
                            { "@odata.count", 1 },
                            { "value", newArray },
                        };

                        var serialized = newObject.ToJsonString();

                        var newExample = new OpenApiString(serialized);

                        content.Value.Example = newExample;

                        var currentSchema = content.Value.Schema;
                        var key = currentSchema.Reference.Id;

                        var keySet = key + "Set";
                        OpenApiSchema newCollectionReference;
                        if (!context.SchemaRepository.Schemas.ContainsKey(keySet))
                        {
                            var newCollectionSchema = new OpenApiSchema()
                            {
                                Title = keySet,
                                AdditionalPropertiesAllowed = false,
                                Properties = new Dictionary<string, OpenApiSchema>()
                                {
                                    { "@odata.count", new OpenApiSchema() { Type = "integer", Description = "The number of items in the result set." } },
                                    { "value", new OpenApiSchema() { Type = "array", Items = currentSchema } },
                                },
                                Required = { "value" },
                                Type = "object",
                            };

                            newCollectionReference = context.SchemaRepository.AddDefinition(newCollectionSchema.Title, newCollectionSchema);
                        }
                        else
                        {
                            newCollectionReference = new()
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = keySet,
                                }
                            };
                        }

                        content.Value.Schema = newCollectionReference;
                    }
                }
            }
        }
    }
}
