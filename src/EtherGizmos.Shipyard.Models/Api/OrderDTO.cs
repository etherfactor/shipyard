using AutoMapper;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class OrderDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public string? Source { get; set; }

    public string? OrderNumber { get; set; }

    public DateOnly? OrderedOn { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? OrderUrl { get; set; }

    public DateOnly? ExpectedShipStartOn { get; set; }

    public DateOnly? ExpectedShipEndOn { get; set; }

    public List<OrderItemDTO> OrderItems { get; set; } = [];
}

public class OrderDTOProfile : Profile
{
    public OrderDTOProfile() : base(nameof(OrderDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<Order, OrderDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        /*  End Audit  */
        toDto.MapMember(dest => dest.Source, src => src.Source);
        toDto.MapMember(dest => dest.OrderNumber, src => src.OrderNumber);
        toDto.MapMember(dest => dest.OrderedOn, src => src.OrderedOn);
        toDto.MapMember(dest => dest.TotalPrice, src => src.TotalPrice);
        toDto.MapMember(dest => dest.OrderUrl, src => src.OrderUrl);
        toDto.MapMember(dest => dest.ExpectedShipStartOn, src => src.ExpectedShipStartOn);
        toDto.MapMember(dest => dest.ExpectedShipEndOn, src => src.ExpectedShipEndOn);
        toDto.MapMember(dest => dest.OrderItems, src => src.OrderItems, opt => opt.ExplicitExpansion());

        var fromDto = mapper.CreateMap<OrderDTO, Order>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.Source, src => src.Source);
        fromDto.MapMember(dest => dest.OrderNumber, src => src.OrderNumber);
        fromDto.MapMember(dest => dest.OrderedOn, src => src.OrderedOn);
        fromDto.MapMember(dest => dest.TotalPrice, src => src.TotalPrice);
        fromDto.MapMember(dest => dest.OrderUrl, src => src.OrderUrl);
        fromDto.MapMember(dest => dest.ExpectedShipStartOn, src => src.ExpectedShipStartOn);
        fromDto.MapMember(dest => dest.ExpectedShipEndOn, src => src.ExpectedShipEndOn);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class OrderDTOExamples
{
    public static OrderDTO Get { get; } = new()
    {
        Id = 1,
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow,
        Source = "Retailer",
        OrderNumber = "123456",
        OrderedOn = DateOnly.FromDateTime(DateTime.Now),
        TotalPrice = 50,
        OrderUrl = null,
        ExpectedShipStartOn = DateOnly.FromDateTime(DateTime.Now),
        ExpectedShipEndOn = DateOnly.FromDateTime(DateTime.Now),
        OrderItems = null!,
    };

    public static OrderDTO Post { get; } = Get;

    public static OrderDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class OrderDTOExampleGet : IExamplesProvider<OrderDTO>
{
    public OrderDTO GetExamples()
    {
        return OrderDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class OrderDTOExamplePost : IExamplesProvider<OrderDTO>
{
    public OrderDTO GetExamples()
    {
        return OrderDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class OrderDTOExamplePatch : IExamplesProvider<OrderDTO>
{
    public OrderDTO GetExamples()
    {
        return OrderDTOExamples.Patch;
    }
}
