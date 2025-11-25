using EtherGizmos.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace EtherGizmos.Shipyard.Api.Errors;

partial class Error
{
    public static class Validation
    {
        public class InvalidModelState : TypedErrorBase
        {
            private const string _message = "One or more validation errors occurred.";

            public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

            public InvalidModelState()
                : base(ErrorConstants.Code.Validation.ModelStateInvalid,
                      ErrorConstants.RequestTarget.Body,
                      _message)
            {
            }

            public InvalidModelState AddDetail(
                ModelStateDictionary modelState,
                Type modelType,
                HttpContext context)
            {
                //If the model contains an empty key, errors are listed under that node
                if (modelState.ContainsKey(""))
                {
                    var regexExtraProperty = new Regex(@"property.+?'(?'PROP'[^']+)'.+?does not exist.+?type.+?'(?'TYPE'[^']+')");

                    foreach (var error in modelState[""]?.Errors ?? [])
                    {
                        string propertyName;

                        var type = error.Exception?.GetType();

                        //Prefer the error message, but fall back on the exception if necessary
                        var errorMessage = FirstNonEmpty(
                            error.ErrorMessage,
                            error.Exception?.Message)
                            ?? "";

                        switch (error.ErrorMessage)
                        {
                            case var _ when regexExtraProperty.IsMatch(errorMessage):
                                var match = regexExtraProperty.Match(errorMessage);
                                propertyName = match.Groups["PROP"].Value;

                                context.Request.Body.Seek(0, SeekOrigin.Begin);
                                var jsonExtraProperty = JsonDocument.Parse(context.Request.Body);
                                propertyName = GetUnexpectedPath(jsonExtraProperty.RootElement, modelType);
                                break;

                            default:
                                propertyName = null!;
                                break;
                        }

                        var detail = new SelfErrorDetail(this, propertyName, errorMessage);
                        AddDetail(detail);
                    }
                }
                //If the model does not contain an empty key, errors are in the root
                else
                {
                    foreach (var item in modelState)
                    {
                        var propertyName = item.Key;
                        foreach (var error in item.Value.Errors)
                        {
                            //Prefer the error message, but fall back on the exception if necessary
                            var errorMessage = FirstNonEmpty(
                                error.ErrorMessage,
                                error.Exception?.Message)
                                ?? "";

                            var detail = new SelfErrorDetail(this, propertyName, errorMessage);
                            AddDetail(detail);
                        }
                    }
                }

                return this;
            }

            private string? FirstNonEmpty(
                params string?[] values)
            {
                foreach (var value in values)
                {
                    if (!string.IsNullOrWhiteSpace(value))
                        return value;
                }

                return null;
            }

            private string GetUnexpectedPath(
                JsonElement json,
                Type modelType,
                string? currentPath = null)
            {
                foreach (var property in json.EnumerateObject())
                {
                    var propertyName = property.Name.ToFirstUpper();
                    var modelProperty = modelType.GetProperty(propertyName);

                    var thisPath = currentPath is null
                        ? property.Name
                        : $"{currentPath}.{property.Name}";

                    if (modelProperty is not null)
                    {
                        var propertyType = modelProperty.PropertyType;
                        if (propertyType.IsAssignableTo(typeof(IEnumerable<object>)) && property.Value.ValueKind == JsonValueKind.Array)
                        {
                            var elementType = propertyType.IsArray
                                ? propertyType.GetElementType()!
                                : propertyType.GetGenericArguments().First();

                            var index = 0;
                            foreach (var item in property.Value.EnumerateArray())
                            {
                                var maybeUnexpectedPath = GetUnexpectedPath(
                                    item,
                                    elementType,
                                    $"{thisPath}[{index}]");

                                if (maybeUnexpectedPath is not null)
                                {
                                    return maybeUnexpectedPath;
                                }

                                index++;
                            }
                        }
                        else if (propertyType.Namespace?.StartsWith("System") == false)
                        {
                            var maybeUnexpectedPath = GetUnexpectedPath(
                                property.Value,
                                modelProperty.PropertyType,
                                thisPath);

                            if (maybeUnexpectedPath is not null)
                            {
                                return maybeUnexpectedPath;
                            }
                        }
                    }
                    else
                    {
                        return thisPath;
                    }
                }

                return currentPath!;
            }

            private class SelfErrorDetail : TypedErrorDetailBase
            {
                public SelfErrorDetail(
                    TypedErrorBase parent, string propertyPath, string message)
                    : base(parent.Code, propertyPath, message)
                {
                }
            }
        }
    }
}
