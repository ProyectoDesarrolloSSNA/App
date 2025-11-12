using AutoMapper;
using TravelBuddy.Application.Contracts.Ratings;
using TravelBuddy.Destinos;
using TravelBuddy.Ratings;
using static OpenIddict.Abstractions.OpenIddictConstants;

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
        CreateMap<Rating, RatingDto>();
        CreateMap<CreateUpdateRatingDto, Rating>();
    }
}