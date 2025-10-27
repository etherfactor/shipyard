using EtherGizmos.Shipyard.Models.Api.Errors;

namespace EtherGizmos.Shipyard.Api.Services.Middleware;

public class ReturnErrorExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ReturnErrorExceptionMiddleware> _logger;

    public ReturnErrorExceptionMiddleware(
        RequestDelegate next,
        ILogger<ReturnErrorExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ReturnErrorException ex)
        {
            await context.Response.WriteErrorAsync(ex.Error);
        }
    }
}
