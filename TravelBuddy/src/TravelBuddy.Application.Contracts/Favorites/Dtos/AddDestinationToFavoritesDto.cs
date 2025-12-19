using System;
using System.ComponentModel.DataAnnotations;

namespace TravelBuddy.Favorites.Dtos
{
    public class AddDestinationToFavoritesDto
    {
        [Required]
        public Guid DestinationId { get; set; }
    }
}
