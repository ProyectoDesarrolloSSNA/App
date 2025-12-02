using AutoMapper;
using TravelBuddy.Destinos;
using TravelBuddy.Ratings;
using static OpenIddict.Abstractions.OpenIddictConstants;
using TravelBuddy.Administration;
using TravelBuddy.Notifications;
using TravelBuddy.Ratings;
using static OpenIddict.Abstractions.OpenIddictConstants;
using TravelBuddy.ExperienciasViaje;
using TravelBuddy.ExperienciasViaje.Dtos;
using TravelBuddy.Favorites;
using TravelBuddy.Favorites.Dtos;
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
        CreateMap<ApiUsageLog, ApiUsageLogDto>();
        CreateMap<AppNotification, AppNotificationDto>();
        CreateMap<CrearActualizarExperienciaViajeDto, ExperienciaViaje>();
        CreateMap<ExperienciaViaje, ExperienciaViajeDto>()
            .ForMember(dest => dest.DestinoNombre, opt => opt.MapFrom(src => src.Destino.Nombre));
        
        // Mapeo para perfil público
        CreateMap<IdentityUser, PublicProfileDto>();
        
        // Mapeo para favoritos (básico, el mapeo completo se hace en el servicio)
        CreateMap<DestinationFavorite, DestinationFavoriteDto>();
    }
}