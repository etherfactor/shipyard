using AutoMapper;
using EtherGizmos.Shipyard.Models.Api.Enums;
using EtherGizmos.Shipyard.Models.Database;
using EtherGizmos.Shipyard.Models.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Models.Api;

public class CarrierRunbookStepDTO
{
    public StepTypeDTO StepType { get; set; }

    public IDictionary<string, object> Payload { get; set; } = new Dictionary<string, object>();
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
        toDto.MapMember(dest => dest.Payload, src => src.Payload);

        var fromDto = mapper.CreateMap<CarrierRunbookStepDTO, CarrierRunbookStep>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.StepType, src => src.StepType);
        fromDto.MapMember(dest => dest.Payload, src => src.Payload);
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
