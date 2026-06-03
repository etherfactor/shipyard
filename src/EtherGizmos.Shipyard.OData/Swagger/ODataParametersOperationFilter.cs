using Microsoft.AspNetCore.OData.Query;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EtherGizmos.Shipyard.Swagger;

public class ODataParametersOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!context.MethodInfo.GetParameters().Any(e => e.ParameterType.IsAssignableTo(typeof(ODataQueryOptions))))
            return;

        operation.Parameters ??= [];

        operation.Parameters.Add(new OpenApiParameter()
        {
            Name = "$select",
            Description = "Limits the properties to return, reducing payload size (e.g., ?$select=Name,Price)",
            Required = false,
            In = ParameterLocation.Query,
            Schema = new OpenApiSchema()
            {
                Type = JsonSchemaType.String,
            },
        });

        operation.Parameters.Add(new OpenApiParameter()
        {
            Name = "$expand",
            Description = "Fetches related navigation properties or child records inline within the same request (e.g., ?$expand=Orders)",
            Required = false,
            In = ParameterLocation.Query,
            Schema = new OpenApiSchema()
            {
                Type = JsonSchemaType.String,
            },
        });

        if (context.MethodInfo.GetCustomAttribute<ProducesResponseSetAttribute>() is not null)
        {
            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "$filter",
                Description = "Retrieves a specific subset of data by applying a Boolean condition (e.g., ?$filter=Price lt 20)",
                Required = false,
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema()
                {
                    Type = JsonSchemaType.String,
                },
            });

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "$orderby",
                Description = "Sorts the result set in ascending (asc) or descending (desc) order (e.g., ?$orderby=Name desc)",
                Required = false,
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema()
                {
                    Type = JsonSchemaType.String,
                },
            });

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "$top",
                Description = "Limits the number of records returned; acts much like a SQL LIMIT clause (e.g., ?$top=10)",
                Required = false,
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema()
                {
                    Type = JsonSchemaType.Integer,
                },
            });

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "$skip",
                Description = "Offsets the result set by omitting a specific number of items, usually used alongside $top for pagination (e.g., ?$skip=20)",
                Required = false,
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema()
                {
                    Type = JsonSchemaType.Integer,
                },
            });

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "$count",
                Description = "Returns the total count of items in the collection as a separate property in the response",
                Required = false,
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema()
                {
                    Type = JsonSchemaType.Boolean,
                },
            });
        }
    }
}
