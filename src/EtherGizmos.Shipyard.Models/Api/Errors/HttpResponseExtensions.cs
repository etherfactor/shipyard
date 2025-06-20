using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Text.Json;

namespace EtherGizmos.Shipyard.Models.Api.Errors;

public static class HttpResponseExtensions
{
    public static async Task WriteErrorAsync(
        this HttpResponse @this,
        TypedErrorBase error)
    {
        if (@this.HasStarted)
            return;

        var typedError = error;

        @this.StatusCode = 200;
        @this.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = null;
        @this.Headers.Clear();
        @this.Body.SetLength(0);

        @this.StatusCode = (int)typedError.StatusCode;
        @this.ContentType = "application/json; charset=utf-8";

        var wrappedError = typedError.Build();

        var content = JsonSerializer.Serialize(wrappedError);
        await @this.WriteAsync(content);
    }
}
