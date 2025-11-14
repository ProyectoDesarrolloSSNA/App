using System;

namespace TravelBuddy.Ratings.Dtos
{
    public class CreateDestinationRatingDto
    {
        public Guid DestinationId { get; set; }
        public int Score { get; set; }     // 1..5
        public string? Comment { get; set; }
    }
}