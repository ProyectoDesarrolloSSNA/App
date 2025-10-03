using AutoMapper;
using TravelBuddy.Destinos;

namespace TravelBuddy
{
    public class TravelBuddyApplicationAutoMapperProfile : Profile
    {
        public TravelBuddyApplicationAutoMapperProfile()
        {
            CreateMap<Destino, DestinoDto>();
            CreateMap<CreateUpdateDestinoDto, Destino>();
        }
    }
}