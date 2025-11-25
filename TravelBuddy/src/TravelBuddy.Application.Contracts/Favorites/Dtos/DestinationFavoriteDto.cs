using System;
using Volo.Abp.Application.Dtos;

namespace TravelBuddy.Favorites.Dtos
{
    public class DestinationFavoriteDto : EntityDto<Guid>
    {
        public Guid DestinationId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreationTime { get; set; }
        
        // Información adicional del destino (opcional, útil para la UI)
        public string? DestinationName { get; set; }
        public string? DestinationCountry { get; set; }
    }
}
