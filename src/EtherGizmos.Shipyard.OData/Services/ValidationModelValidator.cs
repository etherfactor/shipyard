using EtherGizmos.Common.Utilities.Abstractions;
using EtherGizmos.Shipyard.Models.Api.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EtherGizmos.Shipyard.OData.Services;

internal class ValidationModelValidator<TModel> : IModelValidator<TModel>
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationModelValidator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task ValidateAsync(
        TModel model,
        CancellationToken cancellationToken = default)
    {
        if (!typeof(TModel).Name.EndsWith("DTO"))
            return Task.CompletedTask;

        var validator = _serviceProvider.GetRequiredService<IObjectModelValidator>();
        var metadataProvider = _serviceProvider.GetRequiredService<IModelMetadataProvider>();

        var actionContext = _serviceProvider.GetRequiredService<IActionContextAccessor>()
            .ActionContext;

        if (actionContext is not null)
        {
            validator.Validate(
                actionContext,
                validationState: new ValidationStateDictionary(),
                prefix: string.Empty,
                model: model
            );

            if (!actionContext.ModelState.IsValid)
            {
                //Find the first argument marked with [FromBody]
                var argumentType = actionContext
                    .ActionDescriptor
                    .Parameters
                    .OfType<ControllerParameterDescriptor>()
                    .FirstOrDefault(e =>
                        e.ParameterInfo.GetCustomAttribute<FromBodyAttribute>() is not null)
                    ?.ParameterType;

                //Throw an error for the type of that argument
                if (argumentType is not null)
                {
                    new Error.Validation.InvalidModelState()
                        .AddDetail(actionContext.ModelState, argumentType, actionContext.HttpContext)
                        .Return();
                }
            }
        }

        return Task.CompletedTask;
    }
}
