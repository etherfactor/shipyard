using AutoMapper;
using EtherGizmos.Shipyard.Models.Api.Enums;
using EtherGizmos.Shipyard.Models.Attributes;
using EtherGizmos.Shipyard.Models.Database;
using EtherGizmos.Shipyard.Models.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Models.Api;

public class CarrierStatusRuleDTO
{
    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    [Required, Regex]
    public string Pattern { get; set; } = null!;

    [Required]
    public StatusTypeDTO StatusType { get; set; }

    /// <summary>
    /// Lower numbers take precedence, in the case of a conflict.
    /// </summary>
    public int Priority { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}

public class CarrierStatusRuleDTOProfile : Profile
{
    public CarrierStatusRuleDTOProfile() : base(nameof(CarrierStatusRuleDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<CarrierStatusRule, CarrierStatusRuleDTO>();
        toDto.IgnoreAllMembers();
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        /*  End Audit  */
        toDto.MapMember(dest => dest.Pattern, src => src.Pattern);
        toDto.MapMember(dest => dest.StatusType, src => src.StatusTypeId);
        toDto.MapMember(dest => dest.Priority, src => src.Priority);
        toDto.MapMember(dest => dest.IsActive, src => src.IsActive);

        var fromDto = mapper.CreateMap<CarrierStatusRuleDTO, CarrierStatusRule>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.Pattern, src => src.Pattern);
        fromDto.MapMember(dest => dest.StatusTypeId, src => src.StatusType);
        fromDto.MapMember(dest => dest.Priority, src => src.Priority);
        fromDto.MapMember(dest => dest.IsActive, src => src.IsActive);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class CarrierStatusRuleDTOExamples
{
    public static CarrierStatusRuleDTO Get { get; } = new()
    {
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow,
        Pattern = "^(?i)out for delivery$",
        StatusType = StatusTypeDTO.OutForDelivery,
        Priority = 0,
        IsActive = true,
    };

    public static CarrierStatusRuleDTO Post { get; } = Get;

    public static CarrierStatusRuleDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class CarrierStatusRuleDTOExampleGet : IExamplesProvider<CarrierStatusRuleDTO>
{
    public CarrierStatusRuleDTO GetExamples()
    {
        return CarrierStatusRuleDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class CarrierStatusRuleDTOExamplePost : IExamplesProvider<CarrierStatusRuleDTO>
{
    public CarrierStatusRuleDTO GetExamples()
    {
        return CarrierStatusRuleDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class CarrierStatusRuleDTOExamplePatch : IExamplesProvider<CarrierStatusRuleDTO>
{
    public CarrierStatusRuleDTO GetExamples()
    {
        return CarrierStatusRuleDTOExamples.Patch;
    }
}
