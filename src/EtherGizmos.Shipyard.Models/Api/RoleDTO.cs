using AutoMapper;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class RoleDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public List<UserDTO> Users { get; set; } = [];
}

public class RoleDTOProfile : Profile
{
    public RoleDTOProfile() : base(nameof(RoleDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<Role, RoleDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        /*  End Audit  */
        toDto.MapMember(dest => dest.Name, src => src.Name);
        toDto.MapMember(dest => dest.Description, src => src.Description);
        toDto.MapMember(dest => dest.Users, src => src.Users, opt => opt.ExplicitExpansion());

        var fromDto = mapper.CreateMap<RoleDTO, Role>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.Name, src => src.Name);
        fromDto.MapMember(dest => dest.Description, src => src.Description);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class RoleDTOExamples
{
    public static RoleDTO Get { get; } = new()
    {
        Id = 1,
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow,
        Name = "System Owner",
    };

    public static RoleDTO Post { get; } = Get;

    public static RoleDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class RoleDTOExampleGet : IExamplesProvider<RoleDTO>
{
    public RoleDTO GetExamples()
    {
        return RoleDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class RoleDTOExamplePost : IExamplesProvider<RoleDTO>
{
    public RoleDTO GetExamples()
    {
        return RoleDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class RoleDTOExamplePatch : IExamplesProvider<RoleDTO>
{
    public RoleDTO GetExamples()
    {
        return RoleDTOExamples.Patch;
    }
}
