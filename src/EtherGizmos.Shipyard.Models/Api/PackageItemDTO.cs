using AutoMapper;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class PackageItemDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public int PackageId { get; set; }

    public PackageDTO? Package { get; set; }

    public int? OrderFulfillmentItemId { get; set; }

    public OrderFulfillmentItemDTO? OrderFulfillmentItem { get; set; }

    public string Description { get; set; } = null!;

    public decimal Quantity { get; set; }

    public string? QuantityUnits { get; set; }
}

public class PackageItemDTOProfile : Profile
{
    public PackageItemDTOProfile() : base(nameof(PackageItemDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<PackageItem, PackageItemDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        /*  End Audit  */
        toDto.MapMember(dest => dest.PackageId, src => src.PackageId);
        toDto.MapMember(dest => dest.Package, src => src.Package, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.OrderFulfillmentItemId, src => src.OrderFulfillmentItemId);
        toDto.MapMember(dest => dest.OrderFulfillmentItem, src => src.OrderFulfillmentItem, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.Description, src => src.Description);
        toDto.MapMember(dest => dest.Quantity, src => src.Quantity);
        toDto.MapMember(dest => dest.QuantityUnits, src => src.QuantityUnits);

        var fromDto = mapper.CreateMap<PackageItemDTO, PackageItem>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.PackageId, src => src.PackageId);
        fromDto.MapMember(dest => dest.OrderFulfillmentItemId, src => src.OrderFulfillmentItemId);
        fromDto.MapMember(dest => dest.Description, src => src.Description);
        fromDto.MapMember(dest => dest.Quantity, src => src.Quantity);
        fromDto.MapMember(dest => dest.QuantityUnits, src => src.QuantityUnits);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class PackageItemDTOExamples
{
    public static PackageItemDTO Get { get; } = new()
    {
        Id = 1,
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow,
        PackageId = 1,
        Package = null,
        OrderFulfillmentItemId = 1,
        OrderFulfillmentItem = null,
        Description = "An item",
        Quantity = 1,
        QuantityUnits = "pcs",
    };

    public static PackageItemDTO Post { get; } = Get;

    public static PackageItemDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class PackageItemDTOExampleGet : IExamplesProvider<PackageItemDTO>
{
    public PackageItemDTO GetExamples()
    {
        return PackageItemDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class PackageItemDTOExamplePost : IExamplesProvider<PackageItemDTO>
{
    public PackageItemDTO GetExamples()
    {
        return PackageItemDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class PackageItemDTOExamplePatch : IExamplesProvider<PackageItemDTO>
{
    public PackageItemDTO GetExamples()
    {
        return PackageItemDTOExamples.Patch;
    }
}
