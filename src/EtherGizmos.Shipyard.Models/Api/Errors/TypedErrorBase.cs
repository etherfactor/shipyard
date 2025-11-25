using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace EtherGizmos.Shipyard.Api.Errors;

public abstract class TypedErrorBase
{
    public string Code { get; }

    public string Target { get; }

    public string Message { get; }

    public List<TypedErrorDetailBase> Details { get; } = [];

    public Exception? Exception { get; }

    public abstract HttpStatusCode StatusCode { get; }

    public TypedErrorBase(
        string code,
        string target,
        string message,
        Exception? exception = null)
    {
        Code = code;
        Target = target;
        Message = message;
        Exception = exception;
    }

    protected void AddDetail(
        TypedErrorDetailBase detail)
    {
        Details.Add(detail);
    }

    public ErrorWrapper Build()
    {
        var error = new ErrorWrapper()
        {
            Error = new()
            {
                Code = Code,
                Target = Target,
                Message = Message,
                Details = Details.Select(e => new ErrorDetail()
                {
                    Code = e.Code,
                    Target = e.Target,
                    Message = e.Message,
                }).ToList(),
                InnerError = Exception is not null
                    ? new(Exception)
                    : null,
            },
        };

        return error;
    }
}

public static class TypedErrorBaseExtensions
{
    [DoesNotReturn]
    public static void Return(
        this TypedErrorBase @this)
    {
        throw new ReturnErrorException(@this);
    }
}

public class ReturnErrorException : Exception
{
    public TypedErrorBase Error { get; }

    public ReturnErrorException(
        TypedErrorBase error)
    {
        Error = error;
    }
}
