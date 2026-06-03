using EtherGizmos.Shipyard.Extensions;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Json.Nodes;

namespace EtherGizmos.Shipyard.Swagger;

public class ResponseSetFilter : IOperationFilter
{
    private ThreadLocal<Dictionary<string, OpenApiSchemaReference>> _schemas = new();

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        //Only want to apply this if we are actually returning a result set, otherwise this will break the model
        if (context.MethodInfo.GetCustomAttribute<ProducesResponseSetAttribute>() is null)
            return;

        operation.Responses ??= [];
        foreach (var response in operation.Responses)
        {
            //No content we can edit
            if (response.Value.Content is null)
                continue;

            foreach (var content in response.Value.Content)
            {
                var example = content.Value.Example;
                if (example is null)
                    continue;

                if (example is not null && !example.IsODataError())
                {
                    var cloned = example.DeepClone();

                    var newArray = new JsonArray()
                    {
                        cloned,
                    };

                    var newObject = new JsonObject()
                    {
                        { "@odata.count", 1 },
                        { "value", newArray },
                    };

                    content.Value.Example = newObject;

                    var currentSchema = (content.Value.Schema as OpenApiSchemaReference)!;
                    var key = currentSchema.Reference.Id!;

                    _schemas.Value ??= new();
                    if (!_schemas.Value.ContainsKey(key))
                    {
                        var keySet = key + "Set";

                        var newCollectionSchema = new OpenApiSchema()
                        {
                            Title = keySet,
                            AdditionalPropertiesAllowed = false,
                            Properties = new Dictionary<string, IOpenApiSchema>()
                            {
                                { "@odata.count", new OpenApiSchema() { Type = JsonSchemaType.Integer, Description = "The number of items in the result set." } },
                                { "value", new OpenApiSchema() { Type = JsonSchemaType.Array, Items = currentSchema } },
                            },
                            Required = new HashSet<string>() { "value" },
                            Type = JsonSchemaType.Object,
                        };

                        var newCollectionReference = context.SchemaRepository.AddDefinition(newCollectionSchema.Title, newCollectionSchema);
                        _schemas.Value.TryAdd(key, newCollectionReference);
                    }

                    var schema = _schemas.Value[key];
                    content.Value.Schema = schema;
                }
            }
        }
    }
}
