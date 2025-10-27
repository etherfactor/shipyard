namespace EtherGizmos.Shipyard.Api.Errors;

public class TypedErrorDetailBase
{
    public string Code { get; }

    public string Target { get; }

    public string Message { get; }

    public TypedErrorDetailBase(
        string code,
        string target,
        string message)
    {
        Code = code;
        Target = target;
        Message = message;
    }
}
