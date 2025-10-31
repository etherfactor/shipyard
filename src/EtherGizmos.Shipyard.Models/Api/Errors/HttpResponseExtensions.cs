using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace EtherGizmos.Shipyard.Api.Errors;

public static class HttpResponseExtensions
{
    public static async Task WriteErrorAsync(
        this HttpResponse @this,
        TypedErrorBase error)
    {
        if (@this.HasStarted)
            return;

        var typedError = error;

        @this.Clear();

        @this.StatusCode = (int)typedError.StatusCode;
        @this.ContentType = "application/json; charset=utf-8";

        var wrappedError = typedError.Build();

        var content = JsonSerializer.Serialize(wrappedError);
        await @this.WriteAsync(content);
    }
}
