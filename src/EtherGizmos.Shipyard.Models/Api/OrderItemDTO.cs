using AutoMapper;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class OrderItemDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public int OrderId { get; set; }

    public OrderDTO? Order { get; set; }

    public string Description { get; set; } = null!;

    public int Quantity { get; set; }

    public string? QuantityUnits { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }
}

public class OrderItemDTOProfile : Profile
{
    public OrderItemDTOProfile() : base(nameof(OrderItemDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<OrderItem, OrderItemDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        /*  End Audit  */
        toDto.MapMember(dest => dest.OrderId, src => src.OrderId);
        toDto.MapMember(dest => dest.Order, src => src.Order, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.Description, src => src.Description);
        toDto.MapMember(dest => dest.Quantity, src => src.Quantity);
        toDto.MapMember(dest => dest.QuantityUnits, src => src.QuantityUnits);
        toDto.MapMember(dest => dest.UnitPrice, src => src.UnitPrice);
        toDto.MapMember(dest => dest.TotalPrice, src => src.TotalPrice);

        var fromDto = mapper.CreateMap<OrderItemDTO, OrderItem>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.OrderId, src => src.OrderId);
        fromDto.MapMember(dest => dest.Description, src => src.Description);
        fromDto.MapMember(dest => dest.Quantity, src => src.Quantity);
        fromDto.MapMember(dest => dest.QuantityUnits, src => src.QuantityUnits);
        fromDto.MapMember(dest => dest.UnitPrice, src => src.UnitPrice);
        fromDto.MapMember(dest => dest.TotalPrice, src => src.TotalPrice);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class OrderItemDTOExamples
{
    public static OrderItemDTO Get { get; } = new()
    {
        Id = 1,
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow,
        OrderId = 1,
        Order = null,
        Description = "An item",
        Quantity = 1,
        QuantityUnits = "pcs",
        UnitPrice = 50,
        TotalPrice = 50,
    };

    public static OrderItemDTO Post { get; } = Get;

    public static OrderItemDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class OrderItemDTOExampleGet : IExamplesProvider<OrderItemDTO>
{
    public OrderItemDTO GetExamples()
    {
        return OrderItemDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class OrderItemDTOExamplePost : IExamplesProvider<OrderItemDTO>
{
    public OrderItemDTO GetExamples()
    {
        return OrderItemDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class OrderItemDTOExamplePatch : IExamplesProvider<OrderItemDTO>
{
    public OrderItemDTO GetExamples()
    {
        return OrderItemDTOExamples.Patch;
    }
}
