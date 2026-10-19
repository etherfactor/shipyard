using AutoMapper;
using EtherGizmos.Shipyard.Api.Enums;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class OrderFulfillmentDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public int OrderId { get; set; }

    public OrderDTO? Order { get; set; }

    public string? Sender { get; set; }

    public OrderFulfillmentStatusTypeDTO OrderFulfillmentStatusType { get; set; }

    public DateOnly? ShippedOn { get; set; }

    public DateTimeOffset? ReceivedAt { get; set; }

    public Guid? ReceivedByUserId { get; set; }

    public UserDTO? ReceivedByUser { get; set; }

    public DateTimeOffset? ResolvedAt { get; set; }

    public Guid? ResolvedByUserId { get; set; }

    public UserDTO? ResolvedByUser { get; set; }

    public string? ResolvedReason { get; set; }

    public List<OrderFulfillmentItemDTO> OrderFulfillmentItems { get; set; } = [];
}

public class OrderFulfillmentDTOProfile : Profile
{
    public OrderFulfillmentDTOProfile() : base(nameof(OrderFulfillmentDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<OrderFulfillment, OrderFulfillmentDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        /*  End Audit  */
        toDto.MapMember(dest => dest.OrderId, src => src.OrderId);
        toDto.MapMember(dest => dest.Order, src => src.Order, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.Sender, src => src.Sender);
        toDto.MapMember(dest => dest.OrderFulfillmentStatusType, src => src.OrderFulfillmentStatusType);
        toDto.MapMember(dest => dest.ShippedOn, src => src.ShippedOn);
        toDto.MapMember(dest => dest.ReceivedAt, src => src.ReceivedAt);
        toDto.MapMember(dest => dest.ReceivedByUserId, src => src.ReceivedByUserId);
        toDto.MapMember(dest => dest.ReceivedByUser, src => src.ReceivedByUser, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.ResolvedAt, src => src.ResolvedAt);
        toDto.MapMember(dest => dest.ResolvedByUserId, src => src.ResolvedByUserId);
        toDto.MapMember(dest => dest.ResolvedByUser, src => src.ResolvedByUser, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.ResolvedReason, src => src.ResolvedReason);
        toDto.MapMember(dest => dest.OrderFulfillmentItems, src => src.OrderFulfillmentItems, opt => opt.ExplicitExpansion());

        var fromDto = mapper.CreateMap<OrderFulfillmentDTO, OrderFulfillment>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.OrderId, src => src.OrderId);
        fromDto.MapMember(dest => dest.Sender, src => src.Sender);
        fromDto.MapMember(dest => dest.OrderFulfillmentStatusType, src => src.OrderFulfillmentStatusType);
        fromDto.MapMember(dest => dest.ShippedOn, src => src.ShippedOn);
        fromDto.MapMember(dest => dest.ReceivedAt, src => src.ReceivedAt);
        fromDto.MapMember(dest => dest.ResolvedAt, src => src.ResolvedAt);
        fromDto.MapMember(dest => dest.ResolvedReason, src => src.ResolvedReason);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class OrderFulfillmentDTOExamples
{
    public static OrderFulfillmentDTO Get { get; } = new()
    {
        Id = 1,
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow,
        OrderId = 1,
        Order = null,
        Sender = "Shipper",
        OrderFulfillmentStatusType = OrderFulfillmentStatusTypeDTO.Pending,
        ShippedOn = null,
        ReceivedAt = null,
        ReceivedByUserId = null,
        ReceivedByUser = null,
        ResolvedAt = null,
        ResolvedByUserId = null,
        ResolvedByUser = null,
        ResolvedReason = null,
        OrderFulfillmentItems = null!,
    };

    public static OrderFulfillmentDTO Post { get; } = Get;

    public static OrderFulfillmentDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class OrderFulfillmentDTOExampleGet : IExamplesProvider<OrderFulfillmentDTO>
{
    public OrderFulfillmentDTO GetExamples()
    {
        return OrderFulfillmentDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class OrderFulfillmentDTOExamplePost : IExamplesProvider<OrderFulfillmentDTO>
{
    public OrderFulfillmentDTO GetExamples()
    {
        return OrderFulfillmentDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class OrderFulfillmentDTOExamplePatch : IExamplesProvider<OrderFulfillmentDTO>
{
    public OrderFulfillmentDTO GetExamples()
    {
        return OrderFulfillmentDTOExamples.Patch;
    }
}
