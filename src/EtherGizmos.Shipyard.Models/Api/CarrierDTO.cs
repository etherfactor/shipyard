namespace EtherGizmos.Shipyard.Models.Api;

public class CarrierDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;
}
