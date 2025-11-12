using System;
using Volo.Abp.Application.Dtos;

namespace TravelBuddy.Ratings.Dtos
{
    public class DestinationRatingDto : EntityDto<Guid>
    {
        public Guid DestinationId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreationTime { get; set; }
    }
}