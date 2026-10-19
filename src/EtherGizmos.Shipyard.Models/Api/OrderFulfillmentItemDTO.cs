using AutoMapper;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class OrderFulfillmentItemDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public int OrderFulfillmentId { get; set; }

    public OrderFulfillmentDTO? OrderFulfillment { get; set; }

    public int OrderItemId { get; set; }

    public OrderItemDTO? OrderItem { get; set; }

    public decimal ExpectedQuantity { get; set; }

    public decimal? ReceivedQuantity { get; set; }
}

public class OrderFulfillmentItemDTOProfile : Profile
{
    public OrderFulfillmentItemDTOProfile() : base(nameof(OrderFulfillmentItemDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<OrderFulfillmentItem, OrderFulfillmentItemDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        /*  End Audit  */
        toDto.MapMember(dest => dest.OrderFulfillmentId, src => src.OrderFulfillmentId);
        toDto.MapMember(dest => dest.OrderFulfillment, src => src.OrderFulfillment, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.OrderItemId, src => src.OrderItemId);
        toDto.MapMember(dest => dest.OrderItem, src => src.OrderItem, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.ExpectedQuantity, src => src.ExpectedQuantity);
        toDto.MapMember(dest => dest.ReceivedQuantity, src => src.ReceivedQuantity);

        var fromDto = mapper.CreateMap<OrderFulfillmentItemDTO, OrderFulfillmentItem>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.OrderFulfillmentId, src => src.OrderFulfillmentId);
        fromDto.MapMember(dest => dest.OrderItemId, src => src.OrderItemId);
        fromDto.MapMember(dest => dest.ExpectedQuantity, src => src.ExpectedQuantity);
        fromDto.MapMember(dest => dest.ReceivedQuantity, src => src.ReceivedQuantity);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class OrderFulfillmentItemDTOExamples
{
    public static OrderFulfillmentItemDTO Get { get; } = new()
    {
        Id = 1,
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow,
        OrderFulfillmentId = 1,
        OrderFulfillment = null,
        OrderItemId = 1,
        OrderItem = null,
        ExpectedQuantity = 1,
        ReceivedQuantity = null,
    };

    public static OrderFulfillmentItemDTO Post { get; } = Get;

    public static OrderFulfillmentItemDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class OrderFulfillmentItemDTOExampleGet : IExamplesProvider<OrderFulfillmentItemDTO>
{
    public OrderFulfillmentItemDTO GetExamples()
    {
        return OrderFulfillmentItemDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class OrderFulfillmentItemDTOExamplePost : IExamplesProvider<OrderFulfillmentItemDTO>
{
    public OrderFulfillmentItemDTO GetExamples()
    {
        return OrderFulfillmentItemDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class OrderFulfillmentItemDTOExamplePatch : IExamplesProvider<OrderFulfillmentItemDTO>
{
    public OrderFulfillmentItemDTO GetExamples()
    {
        return OrderFulfillmentItemDTOExamples.Patch;
    }
}
