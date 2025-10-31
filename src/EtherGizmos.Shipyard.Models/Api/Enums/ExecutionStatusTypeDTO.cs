namespace EtherGizmos.Shipyard.Api.Enums;

public enum ExecutionStatusTypeDTO
{
    Queued = 1,
    Running = 10,
    Successful = 100,
    Failed = -100,
    TimedOut = -10,
    Cancelled = -20,
}
