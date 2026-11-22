using AutoMapper;
using Domain;
using Services.Dtos;

namespace ApiSampleFinal.Automapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Granja, GranjaResponse>();
        CreateMap<Cow, VacaProduccionResponse>()
            .ForMember(d => d.VacaId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Raza, o => o.MapFrom(s => s.Race));
        CreateMap<LoteProducto, LoteInventarioResponse>()
            .ForMember(d => d.TipoProducto, o => o.MapFrom(s => s.Tipo));
    }
}
