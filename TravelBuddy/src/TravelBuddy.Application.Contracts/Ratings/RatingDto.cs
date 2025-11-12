using System;

namespace TravelBuddy.Application.Contracts.Ratings
{
    public class RatingDto
    {
        public Guid Id { get; set; }
        public Guid DestinationId { get; set; }
        public Guid UserId { get; set; }
        public int Stars { get; set; }
        public string? Comment { get; set; }
    }

    public class CreateUpdateRatingDto
    {
        public Guid DestinationId { get; set; }
        public Guid UserId { get; set; }
        public int Stars { get; set; }
        public string? Comment { get; set; }
    }
}
