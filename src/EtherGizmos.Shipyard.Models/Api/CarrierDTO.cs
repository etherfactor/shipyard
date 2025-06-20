using AutoMapper;
using EtherGizmos.Shipyard.Models.Database;
using EtherGizmos.Shipyard.Models.Extensions;

namespace EtherGizmos.Shipyard.Models.Api;

public class CarrierDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;
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

        var fromDto = mapper.CreateMap<CarrierDTO, Carrier>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.Name, src => src.Name);
        fromDto.MapMember(dest => dest.Slug, src => src.Slug);
    })
    { }
}
