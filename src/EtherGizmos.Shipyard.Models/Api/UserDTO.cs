using AutoMapper;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class UserDTO
{
    public Guid Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    public string? EmailAddress { get; set; }

    public string? GivenName { get; set; }

    public string? FamilyName { get; set; }

    public string? FullName { get; set; }

    public int GroupId { get; set; }

    public GroupDTO? Group { get; set; }

    public List<RoleDTO> Roles { get; set; } = [];
}

public class UserDTOProfile : Profile
{
    public UserDTOProfile() : base(nameof(UserDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<User, UserDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        /*  End Audit  */
        toDto.MapMember(dest => dest.Username, src => src.Username);
        toDto.MapMember(dest => dest.Password, src => "***");
        toDto.MapMember(dest => dest.EmailAddress, src => src.EmailAddress);
        toDto.MapMember(dest => dest.GivenName, src => src.GivenName);
        toDto.MapMember(dest => dest.FamilyName, src => src.FamilyName);
        toDto.MapMember(dest => dest.FullName, src => src.FullName);
        toDto.MapMember(dest => dest.GroupId, src => src.GroupId);
        toDto.MapMember(dest => dest.Group, src => src.Group, opt => opt.ExplicitExpansion());

        var fromDto = mapper.CreateMap<UserDTO, User>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.Username, src => src.Username);
        fromDto.MapMember(dest => dest.Password, src => src.Password, opt =>
            opt.Condition(dto => !string.IsNullOrWhiteSpace(dto.Password) && dto.Password != "***"));
        fromDto.MapMember(dest => dest.EmailAddress, src => src.EmailAddress);
        fromDto.MapMember(dest => dest.GivenName, src => src.GivenName);
        fromDto.MapMember(dest => dest.FamilyName, src => src.FamilyName);
        fromDto.MapMember(dest => dest.FullName, src => src.FullName);
        fromDto.MapMember(dest => dest.GroupId, src => src.GroupId);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class UserDTOExamples
{
    public static UserDTO Get { get; } = new()
    {
        Id = Guid.NewGuid(),
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow,
        Username = "admin",
        Password = "***",
        GroupId = 1,
    };

    public static UserDTO Post { get; } = Get;

    public static UserDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class UserDTOExampleGet : IExamplesProvider<UserDTO>
{
    public UserDTO GetExamples()
    {
        return UserDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class UserDTOExamplePost : IExamplesProvider<UserDTO>
{
    public UserDTO GetExamples()
    {
        return UserDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class UserDTOExamplePatch : IExamplesProvider<UserDTO>
{
    public UserDTO GetExamples()
    {
        return UserDTOExamples.Patch;
    }
}
