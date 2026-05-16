using AutoMapper;
using EtherGizmos.Common.Converters;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace EtherGizmos.Shipyard.Api;

public class DynamicBagDTO
{
    public string DataRaw
    {
        get => JsonSerializer.Serialize(Data);
        set => Data = JsonSerializer.Deserialize<IDictionary<string, object?>>(value, new JsonSerializerOptions()
        {
            Converters =
            {
                new ObjectToInferredTypesConverter(),
            },
        })!;
    }

    public IDictionary<string, object?> Data { get; set; } = new Dictionary<string, object?>();
}

public class DynamicBagDTOProfile : Profile
{
    public DynamicBagDTOProfile() : base(nameof(DynamicBagDTOProfile), mapper =>
    {
        //Nothing for now?
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class DynamicBagDTOExamples
{
    public static DynamicBagDTO Get { get; } = new()
    {
        Data = new Dictionary<string, object?>()
        {
            ["key"] = "value",
        },
    };

    public static DynamicBagDTO Post { get; } = Get;

    public static DynamicBagDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class DynamicBagDTOExampleGet : IExamplesProvider<DynamicBagDTO>
{
    public DynamicBagDTO GetExamples()
    {
        return DynamicBagDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class DynamicBagDTOExamplePost : IExamplesProvider<DynamicBagDTO>
{
    public DynamicBagDTO GetExamples()
    {
        return DynamicBagDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class DynamicBagDTOExamplePatch : IExamplesProvider<DynamicBagDTO>
{
    public DynamicBagDTO GetExamples()
    {
        return DynamicBagDTOExamples.Patch;
    }
}
