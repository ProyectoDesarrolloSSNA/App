using AutoMapper;
using TravelBuddy.Ratings;
using TravelBuddy.Ratings.Dtos;

namespace TravelBuddy.Application.Ratings
{
    public class RatingApplicationAutoMapperProfile : Profile
    {
        public RatingApplicationAutoMapperProfile()
        {
            CreateMap<DestinationRating, DestinationRatingDto>();
        }
    }
}