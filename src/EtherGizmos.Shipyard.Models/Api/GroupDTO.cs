using AutoMapper;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class GroupDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public List<UserDTO> Users { get; set; } = [];
}

public class GroupDTOProfile : Profile
{
    public GroupDTOProfile() : base(nameof(GroupDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<Group, GroupDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        /*  End Audit  */
        toDto.MapMember(dest => dest.Name, src => src.Name);
        toDto.MapMember(dest => dest.Description, src => src.Description);
        toDto.MapMember(dest => dest.Users, src => src.Users, opt => opt.ExplicitExpansion());

        var fromDto = mapper.CreateMap<GroupDTO, Group>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.Name, src => src.Name);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class GroupDTOExamples
{
    public static GroupDTO Get { get; } = new()
    {
        Id = 1,
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow,
        Name = "Default",
    };

    public static GroupDTO Post { get; } = Get;

    public static GroupDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class GroupDTOExampleGet : IExamplesProvider<GroupDTO>
{
    public GroupDTO GetExamples()
    {
        return GroupDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class GroupDTOExamplePost : IExamplesProvider<GroupDTO>
{
    public GroupDTO GetExamples()
    {
        return GroupDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class GroupDTOExamplePatch : IExamplesProvider<GroupDTO>
{
    public GroupDTO GetExamples()
    {
        return GroupDTOExamples.Patch;
    }
}
