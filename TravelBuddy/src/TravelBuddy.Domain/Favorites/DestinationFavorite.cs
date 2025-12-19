using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace TravelBuddy.Favorites
{
    /// <summary>
    /// Representa un destino marcado como favorito por un usuario
    /// </summary>
    public class DestinationFavorite : CreationAuditedAggregateRoot<Guid>, IUserOwned
    {
        public Guid DestinationId { get; private set; }
        public Guid UserId { get; set; }

        // Constructor protegido para EF Core
        protected DestinationFavorite() { }

        public DestinationFavorite(
            Guid id,
            Guid destinationId,
            Guid userId) : base(id)
        {
            DestinationId = destinationId;
            UserId = userId;
        }
    }
}
