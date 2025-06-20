using EtherGizmos.Shipyard.Models.Api.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OData.Extensions;
using System.Reflection;

namespace EtherGizmos.Shipyard.OData.Services.Filters;

public class ModelStateActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            //Find the first argument marked with [FromBody]
            var argumentType = context
                .ActionDescriptor
                .Parameters
                .OfType<ControllerParameterDescriptor>()
                .FirstOrDefault(e =>
                    e.ParameterInfo.GetCustomAttribute<FromBodyAttribute>() is not null)
                ?.ParameterType;

            var oDataFeature = context.HttpContext.ODataFeature();

            //Throw an error for the type of that argument
            if (argumentType is not null)
            {
                new Error.Validation.InvalidModelState()
                    .AddDetail(context.ModelState, argumentType, context.HttpContext)
                    .Return();
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        return;
    }
}
