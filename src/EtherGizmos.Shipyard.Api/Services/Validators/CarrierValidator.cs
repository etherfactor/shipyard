using AutoMapper;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Api.Enums;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Api.Services.Validators;

internal class CarrierValidator : IModelValidator<Carrier>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public CarrierValidator(
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        IModelValidatorFactory validatorFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public Task ValidateAsync(
        Carrier model,
        CancellationToken cancellationToken = default)
    {
        var carrier = (CarrierDTO)_mapper
            .MapExplicitly(model)
            .To<CarrierDTO>()
            .Execute();

        var modelState = new ModelStateDictionary();
        var context = new ValidationContext(carrier);
        var results = new List<ValidationResult>();

        var returnError = new Error.Validation.InvalidModelState();

        var isValid = Validator.TryValidateObject(carrier, context, results, true);
        if (!isValid)
        {
            foreach (var error in results)
            {
                foreach (var memberName in error.MemberNames)
                {
                    modelState.AddModelError(memberName, error.ErrorMessage!);
                }
            }

            returnError.AddDetail(modelState, typeof(CarrierDTO), _httpContextAccessor.HttpContext);
        }

        for (var i = 0; i < carrier.Steps.Count; i++)
        {
            var step = carrier.Steps[i];
            step.Payload.TryGetValue("from", out var from);
            step.Payload.TryGetValue("isRegex", out var isRegex);
            step.Payload.TryGetValue("name", out var name);
            step.Payload.TryGetValue("script", out var script);
            step.Payload.TryGetValue("selector", out var selector);
            step.Payload.TryGetValue("to", out var to);
            step.Payload.TryGetValue("trim", out var trim);
            step.Payload.TryGetValue("url", out var url);
            step.Payload.TryGetValue("value", out var value);
            step.Payload.TryGetValue("var", out var var);

            var path = $"steps[{i}].";
            switch (step.StepType)
            {
                case StepTypeDTO.Click:
                    if (!IsValid(selector)) AddRequiredFor(returnError, path, "selector");
                    break;

                case StepTypeDTO.Extract:
                    if (!IsValid(selector)) AddRequiredFor(returnError, path, "selector");
                    if (!IsValid(var)) AddRequiredFor(returnError, path, "var");
                    if (!IsValid(trim)) AddRequiredFor(returnError, path, "trim");
                    break;

                case StepTypeDTO.Navigate:
                    if (!IsValid(url)) AddRequiredFor(returnError, path, "url");
                    break;

                case StepTypeDTO.Replace:
                    if (!IsValid(var)) AddRequiredFor(returnError, path, "var");
                    if (!IsValid(from)) AddRequiredFor(returnError, path, "from");
                    if (!IsValid(to)) AddRequiredFor(returnError, path, "to");
                    if (!IsValid(isRegex)) AddRequiredFor(returnError, path, "isRegex");
                    if (!IsValid(trim)) AddRequiredFor(returnError, path, "trim");
                    break;

                case StepTypeDTO.Script:
                    if (!IsValid(script)) AddRequiredFor(returnError, path, "script");
                    break;

                case StepTypeDTO.Send:
                    if (!IsValid(selector)) AddRequiredFor(returnError, path, "selector");
                    if (!IsValid(value)) AddRequiredFor(returnError, path, "value");
                    break;

                case StepTypeDTO.Set:
                    if (!IsValid(var)) AddRequiredFor(returnError, path, "var");
                    if (!IsValid(value)) AddRequiredFor(returnError, path, "value");
                    if (!IsValid(trim)) AddRequiredFor(returnError, path, "trim");
                    break;

                case StepTypeDTO.WaitFor:
                    if (!IsValid(selector)) AddRequiredFor(returnError, path, "selector");
                    break;
            }
        }

        for (var i = 0; i < carrier.Rules.Count; i++)
        {
            var rule = carrier.Rules[i];

            var pattern = rule.Pattern;
            var statusType = rule.StatusType;
            var priority = rule.Priority;

            var path = $"rules[{i}].";
            if (!IsValid(pattern)) AddRequiredFor(returnError, pattern, "pattern");
            if (!IsValid(statusType)) AddRequiredFor(returnError, pattern, "statusType");
            if (!IsValid(priority)) AddRequiredFor(returnError, path, "priority");
        }

        if (returnError.Details.Count > 0)
        {
            returnError.Return();
        }

        return Task.CompletedTask;
    }

    private bool IsValid(
        object? value)
    {
        if (value is string stringValue)
        {
            return !string.IsNullOrWhiteSpace(stringValue);
        }
        else
        {
            return value is not null;
        }
    }

    private void AddRequiredFor(
        Error.Validation.InvalidModelState error,
        string path,
        string name)
    {
        error.AddDetail($"{path}{name}", $"The '{name}' field is required.");
    }
}
