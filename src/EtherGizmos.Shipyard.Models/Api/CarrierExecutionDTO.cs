using AutoMapper;
using EtherGizmos.Shipyard.Api.Enums;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class CarrierExecutionDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public int? CarrierId { get; set; }

    public CarrierDTO? Carrier { get; set; }

    public int? PackageId { get; set; }

    public PackageDTO? Package { get; set; }

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public ExecutionStatusTypeDTO ExecutionStatusType { get; set; }

    public short StepCount { get; set; }

    public short? FailureStepIndex { get; set; }

    public List<CarrierExecutionArtifactDTO> Artifacts { get; set; } = [];
}

public class CarrierExecutionDTOProfile : Profile
{
    public CarrierExecutionDTOProfile() : base(nameof(CarrierExecutionDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<CarrierExecution, CarrierExecutionDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        /*  End Audit  */
        toDto.MapMember(dest => dest.CarrierId, src => src.CarrierId);
        toDto.MapMember(dest => dest.Carrier, src => src.Carrier, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.PackageId, src => src.PackageId);
        toDto.MapMember(dest => dest.Package, src => src.Package, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.StartedAt, src => src.StartedAt);
        toDto.MapMember(dest => dest.CompletedAt, src => src.CompletedAt);
        toDto.MapMember(dest => dest.ExecutionStatusType, src => src.ExecutionStatus);
        toDto.MapMember(dest => dest.StepCount, src => src.StepCount);
        toDto.MapMember(dest => dest.FailureStepIndex, src => src.FailureStepIndex);
        toDto.MapMember(dest => dest.Artifacts, src => src.Artifacts);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class CarrierExecutionDTOExamples
{
    public static CarrierExecutionDTO Get { get; } = new()
    {
        Id = 1,
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow,
        CarrierId = 1,
        StartedAt = null,
        CompletedAt = null,
        ExecutionStatusType = ExecutionStatusTypeDTO.Queued,
        StepCount = 1,
        FailureStepIndex = null,
        Artifacts = [CarrierExecutionArtifactDTOExamples.Get],
    };

    public static CarrierExecutionDTO Post { get; } = Get;

    public static CarrierExecutionDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class CarrierExecutionDTOExampleGet : IExamplesProvider<CarrierExecutionDTO>
{
    public CarrierExecutionDTO GetExamples()
    {
        return CarrierExecutionDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class CarrierExecutionDTOExamplePost : IExamplesProvider<CarrierExecutionDTO>
{
    public CarrierExecutionDTO GetExamples()
    {
        return CarrierExecutionDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class CarrierExecutionDTOExamplePatch : IExamplesProvider<CarrierExecutionDTO>
{
    public CarrierExecutionDTO GetExamples()
    {
        return CarrierExecutionDTOExamples.Patch;
    }
}
