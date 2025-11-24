using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace TravelBuddy.Ratings
{
    public class Rating : FullAuditedAggregateRoot<Guid>, IUserOwned
    {
        public Guid DestinationId { get; set; }
        public Guid UserId { get; set; }
        public int Stars { get; set; } // 1-5
        public string? Comment { get; set; }
    }
}
