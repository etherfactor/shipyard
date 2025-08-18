using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace EtherGizmos.Common;

internal class FormValueRequiredAttribute : ActionMethodSelectorAttribute
{
    private readonly string _name;

    public FormValueRequiredAttribute(
        string name)
    {
        _name = name;
    }

    public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
    {
        if (!routeContext.HttpContext.Request.Method.Equals(HttpMethod.Post.Method, StringComparison.OrdinalIgnoreCase))
            return false;

        if (routeContext.HttpContext.Request.ContentType is null)
            return false;

        if (!routeContext.HttpContext.Request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
            return false;

        return !string.IsNullOrWhiteSpace(routeContext.HttpContext.Request.Form[_name]);
    }
}
