using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelBuddy.Favorites.Dtos;
using Volo.Abp.Application.Services;

namespace TravelBuddy.Favorites
{
    public interface IDestinationFavoriteAppService : IApplicationService
    {
        /// <summary>
        /// Agrega un destino a la lista de favoritos del usuario actual
        /// </summary>
        Task<DestinationFavoriteDto> AddToFavoritesAsync(AddDestinationToFavoritesDto input);

        /// <summary>
        /// Elimina un destino de la lista de favoritos del usuario actual
        /// </summary>
        Task RemoveFromFavoritesAsync(Guid destinationId);

        /// <summary>
        /// Obtiene la lista personal de favoritos del usuario actual
        /// </summary>
        Task<List<DestinationFavoriteDto>> GetMyFavoritesAsync();

        /// <summary>
        /// Verifica si un destino está en los favoritos del usuario actual
        /// </summary>
        Task<bool> IsInFavoritesAsync(Guid destinationId);
    }
}
