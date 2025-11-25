using System;
using Volo.Abp.Application.Dtos;

namespace TravelBuddy.Ratings.Dtos
{
    public class DestinationRatingDto : EntityDto<Guid>
    {
        public Guid DestinationId { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; } // Nombre del usuario que calificó
        public int Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid UserId { get; set; }
    }
}