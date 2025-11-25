using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TravelBuddy.Ratings.Dtos
{
    public interface IDestinationRatingAppService : IApplicationService
    {
        /// <summary>
        /// Crea una nueva calificación para un destino
        /// </summary>
        Task<DestinationRatingDto> CreateAsync(CreateDestinationRatingDto input);

        /// <summary>
        /// Actualiza una calificación propia
        /// </summary>
        Task<DestinationRatingDto> UpdateAsync(Guid id, UpdateDestinationRatingDto input);

        /// <summary>
        /// Elimina una calificación propia
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Obtiene el promedio de calificaciones de un destino
        /// </summary>
        Task<DestinationRatingAverageDto> GetAverageRatingAsync(Guid destinationId);

        /// <summary>
        /// Lista todas las calificaciones y comentarios de un destino
        /// </summary>
        Task<List<DestinationRatingDto>> GetAllByDestinationAsync(Guid destinationId);

        /// <summary>
        /// Obtiene las calificaciones propias del usuario actual
        /// </summary>
        Task<List<DestinationRatingDto>> GetMyRatingsAsync(Guid destinationId);

        /// <summary>
        /// Obtiene la calificación del usuario actual para un destino específico (si existe)
        /// Útil para saber si ya calificó y obtener el ID del rating para editar/eliminar
        /// </summary>
        Task<DestinationRatingDto?> GetMyRatingForDestinationAsync(Guid destinationId);
    }
}
