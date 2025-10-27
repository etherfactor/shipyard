using System.Text.Json.Serialization;

namespace EtherGizmos.Shipyard.Models.Api.Errors;

public partial class Error
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = null!;

    [JsonPropertyName("target")]
    public string Target { get; set; } = null!;

    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;

    [JsonPropertyName("details"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ErrorDetail>? Details { get; set; }

    [JsonPropertyName("innererror"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ErrorInner? InnerError { get; set; }
}

public class ErrorDetail
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = null!;

    [JsonPropertyName("target")]
    public string Target { get; set; } = null!;

    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;
}

public class ErrorInner
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;

    [JsonPropertyName("stacktrace"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StackTrace { get; set; }

    [JsonPropertyName("internalexception"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ErrorInner? InnerError { get; set; }

    [JsonPropertyName("internalexceptions"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ErrorInner>? InnerErrors { get; set; }

    [JsonPropertyName("properties"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object?>? Properties { get; set; }

    public ErrorInner() { }

    public ErrorInner(Exception ex)
    {
        Type = ex.GetType().FullName!;
        Message = ex.Message;
        StackTrace = ex.StackTrace;

        if (ex is AggregateException aex)
        {
            if (aex.InnerExceptions.Any())
            {
                InnerErrors = aex
                    .InnerExceptions
                    .Select(x => new ErrorInner(x))
                    .ToList();
            }
        }
        else if (ex.InnerException is not null)
        {
            InnerError = new ErrorInner(ex.InnerException);
        }
    }
}

public class ErrorWrapper
{
    [JsonPropertyName("error")]
    public Error Error { get; set; } = null!;
}
