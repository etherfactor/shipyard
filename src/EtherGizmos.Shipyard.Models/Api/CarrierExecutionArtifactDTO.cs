using AutoMapper;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class CarrierExecutionArtifactDTO
{
    public string ArtifactUri { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public long Bytes { get; set; }

    public short? StepIndex { get; set; }
}

public class CarrierExecutionArtifactDTOProfile : Profile
{
    public CarrierExecutionArtifactDTOProfile() : base(nameof(CarrierExecutionArtifactDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<CarrierExecutionArtifact, CarrierExecutionArtifactDTO>();
        toDto.IgnoreAllMembers();
        /* Begin Audit */
        /*  End Audit  */
        toDto.MapMember(dest => dest.ArtifactUri, src => src.ArtifactUri);
        toDto.MapMember(dest => dest.ContentType, src => src.ContentType);
        toDto.MapMember(dest => dest.FileName, src => src.FileName);
        toDto.MapMember(dest => dest.Bytes, src => src.Bytes);
        toDto.MapMember(dest => dest.StepIndex, src => src.StepIndex);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class CarrierExecutionArtifactDTOExamples
{
    public static CarrierExecutionArtifactDTO Get { get; } = new()
    {
        ArtifactUri = "artifact://runs/1/00000000-0000-0000-0000-000000000000",
        ContentType = "text/plain; charset=utf8",
        FileName = "test.txt",
        Bytes = 255,
        StepIndex = 1,
    };

    public static CarrierExecutionArtifactDTO Post { get; } = Get;

    public static CarrierExecutionArtifactDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class CarrierExecutionArtifactDTOExampleGet : IExamplesProvider<CarrierExecutionArtifactDTO>
{
    public CarrierExecutionArtifactDTO GetExamples()
    {
        return CarrierExecutionArtifactDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class CarrierExecutionArtifactDTOExamplePost : IExamplesProvider<CarrierExecutionArtifactDTO>
{
    public CarrierExecutionArtifactDTO GetExamples()
    {
        return CarrierExecutionArtifactDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class CarrierExecutionArtifactDTOExamplePatch : IExamplesProvider<CarrierExecutionArtifactDTO>
{
    public CarrierExecutionArtifactDTO GetExamples()
    {
        return CarrierExecutionArtifactDTOExamples.Patch;
    }
}
