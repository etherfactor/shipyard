using AutoMapper;
using EtherGizmos.Shipyard.Models.Database;
using EtherGizmos.Shipyard.Models.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Models.Api;

public class CarrierDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Slug { get; set; } = null!;

    [Required]
    public List<CarrierRunbookStepDTO> Steps { get; set; } = [];

    [Required]
    public List<CarrierStatusRuleDTO> Rules { get; set; } = [];
}

public class CarrierDTOProfile : Profile
{
    public CarrierDTOProfile() : base(nameof(CarrierDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<Carrier, CarrierDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        /*  End Audit  */
        toDto.MapMember(dest => dest.Name, src => src.Name);
        toDto.MapMember(dest => dest.Slug, src => src.Slug);
        toDto.MapMember(dest => dest.Steps, src => src.Steps);
        toDto.MapMember(dest => dest.Rules, src => src.Rules);

        var fromDto = mapper.CreateMap<CarrierDTO, Carrier>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.Name, src => src.Name);
        fromDto.MapMember(dest => dest.Slug, src => src.Slug);
        fromDto.MapMember(dest => dest.Steps, src => src.Steps);
        fromDto.MapMember(dest => dest.Rules, src => src.Rules);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class CarrierDTOExamples
{
    public static CarrierDTO Get { get; } = new()
    {
        Id = 1,
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow,
        Name = "USPS",
        Slug = "usps",
        Rules = [CarrierStatusRuleDTOExamples.Get],
    };

    public static CarrierDTO Post { get; } = Get;

    public static CarrierDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class CarrierDTOExampleGet : IExamplesProvider<CarrierDTO>
{
    public CarrierDTO GetExamples()
    {
        return CarrierDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class CarrierDTOExamplePost : IExamplesProvider<CarrierDTO>
{
    public CarrierDTO GetExamples()
    {
        return CarrierDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class CarrierDTOExamplePatch : IExamplesProvider<CarrierDTO>
{
    public CarrierDTO GetExamples()
    {
        return CarrierDTOExamples.Patch;
    }
}
