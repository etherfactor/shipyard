namespace EtherGizmos.Shipyard.Api.Enums;

public enum TrackingStatusTypeDTO
{
    Unknown = 0,
    Waiting = 1,
    InTransit = 10,
    OutForDelivery = 20,
    Delivered = 100,
    FailedAttempt = -10,
    Returned = -100,
    Expired = -200,
}
