using AutoMapper;
using EtherGizmos.Shipyard.Models.Api.Enums;
using EtherGizmos.Shipyard.Models.Database;
using EtherGizmos.Shipyard.Models.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace EtherGizmos.Shipyard.Models.Api;

public class CarrierRunbookStepDTO
{
    public StepTypeDTO StepType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? From { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Selector { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<CarrierRunbookStepDTO>? Steps { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? To { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Trim { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Url { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Value { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Var { get; set; }
}

public class CarrierRunbookStepDTOProfile : Profile
{
    public CarrierRunbookStepDTOProfile() : base(nameof(CarrierRunbookStepDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<CarrierRunbookStep, CarrierRunbookStepDTO>();
        toDto.IgnoreAllMembers();
        /* Begin Audit */
        /*  End Audit  */
        toDto.MapMember(dest => dest.StepType, src => src.StepType);
        toDto.MapMember(dest => dest.From, src => src.From);
        toDto.MapMember(dest => dest.Name, src => src.Name);
        toDto.MapMember(dest => dest.Selector, src => src.Selector);
        toDto.MapMember(dest => dest.Steps, src => src.Steps);
        toDto.MapMember(dest => dest.To, src => src.To);
        toDto.MapMember(dest => dest.Trim, src => src.Trim);
        toDto.MapMember(dest => dest.Url, src => src.Url);
        toDto.MapMember(dest => dest.Value, src => src.Value);
        toDto.MapMember(dest => dest.Var, src => src.Var);

        var fromDto = mapper.CreateMap<CarrierRunbookStepDTO, CarrierRunbookStep>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.StepType, src => src.StepType);
        fromDto.MapMember(dest => dest.From, src => src.From);
        fromDto.MapMember(dest => dest.Name, src => src.Name);
        fromDto.MapMember(dest => dest.Selector, src => src.Selector);
        fromDto.MapMember(dest => dest.Steps, src => src.Steps);
        fromDto.MapMember(dest => dest.To, src => src.To);
        fromDto.MapMember(dest => dest.Trim, src => src.Trim);
        fromDto.MapMember(dest => dest.Url, src => src.Url);
        fromDto.MapMember(dest => dest.Value, src => src.Value);
        fromDto.MapMember(dest => dest.Var, src => src.Var);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class CarrierRunbookStepDTOExamples
{
    public static CarrierRunbookStepDTO Get { get; } = new()
    {
        //TODO
    };

    public static CarrierRunbookStepDTO Post { get; } = Get;

    public static CarrierRunbookStepDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class CarrierRunbookStepDTOExampleGet : IExamplesProvider<CarrierRunbookStepDTO>
{
    public CarrierRunbookStepDTO GetExamples()
    {
        return CarrierRunbookStepDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class CarrierRunbookStepDTOExamplePost : IExamplesProvider<CarrierRunbookStepDTO>
{
    public CarrierRunbookStepDTO GetExamples()
    {
        return CarrierRunbookStepDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class CarrierRunbookStepDTOExamplePatch : IExamplesProvider<CarrierRunbookStepDTO>
{
    public CarrierRunbookStepDTO GetExamples()
    {
        return CarrierRunbookStepDTOExamples.Patch;
    }
}
