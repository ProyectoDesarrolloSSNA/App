using AutoMapper;
using TravelBuddy.Destinos;
using TravelBuddy.Users;
using Volo.Abp.Identity;

namespace TravelBuddy;

public class TravelBuddyApplicationAutoMapperProfile : Profile
{
    public TravelBuddyApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Destino, DestinoDto>();
        CreateMap<CreateUpdateDestinoDto, Destino>();
        
        // Mapeo para perfil público
        CreateMap<IdentityUser, PublicProfileDto>();
    }
}