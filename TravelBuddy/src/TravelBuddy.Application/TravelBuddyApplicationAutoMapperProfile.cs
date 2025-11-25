using AutoMapper;
using TravelBuddy.Application.Contracts.Ratings;
using TravelBuddy.Destinos;
using TravelBuddy.ExperienciasViaje;
using TravelBuddy.ExperienciasViaje.Dtos;
using TravelBuddy.Ratings;
using TravelBuddy.Users;
using Volo.Abp.Identity;

namespace TravelBuddy;

public class TravelBuddyApplicationAutoMapperProfile : Profile
{
    public TravelBuddyApplicationAutoMapperProfile()
    {
        CreateMap<Destino, DestinoDto>();
        CreateMap<CreateUpdateDestinoDto, Destino>();
        CreateMap<Rating, RatingDto>();
        CreateMap<CreateUpdateRatingDto, Rating>();
        CreateMap<CrearActualizarExperienciaViajeDto, ExperienciaViaje>();
        CreateMap<ExperienciaViaje, ExperienciaViajeDto>()
        .ForMember(dest => dest.DestinoNombre, opt => opt.MapFrom(src => src.Destino.Nombre));
        
        // Mapeo para perfil público
        CreateMap<IdentityUser, PublicProfileDto>();
    }
}