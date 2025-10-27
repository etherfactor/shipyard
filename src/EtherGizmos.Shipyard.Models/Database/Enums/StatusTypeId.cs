namespace EtherGizmos.Shipyard.Database.Enums;

public static class StatusTypeId
{
    public const int Unknown = 0;

    public const int Waiting = 1;

    public const int InTransit = 10;

    public const int OutForDelivery = 20;

    public const int Delivered = 100;

    public const int FailedAttempt = -10;

    public const int Returned = -100;

    public const int Expired = -200;
}
