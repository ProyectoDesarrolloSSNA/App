using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelBuddy.Ratings.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace TravelBuddy.HttpApi.Controllers.Ratings
{
    [Route("api/destination-ratings")]
    [ApiController]
    public class DestinationRatingController : AbpControllerBase
    {
        private readonly IDestinationRatingAppService _ratingAppService;

        public DestinationRatingController(IDestinationRatingAppService ratingAppService)
        {
            _ratingAppService = ratingAppService;
        }

        /// <summary>
        /// Crear una nueva calificación
        /// POST: api/destination-ratings
        /// </summary>
        [HttpPost]
        public async Task<DestinationRatingDto> CreateAsync(CreateDestinationRatingDto input)
        {
            return await _ratingAppService.CreateAsync(input);
        }

        /// <summary>
        /// Actualizar una calificación propia
        /// PUT: api/destination-ratings/{id}
        /// </summary>
        [HttpPut("{id}")]
        public async Task<DestinationRatingDto> UpdateAsync(Guid id, UpdateDestinationRatingDto input)
        {
            return await _ratingAppService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Eliminar una calificación propia
        /// DELETE: api/destination-ratings/{id}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _ratingAppService.DeleteAsync(id);
        }

        /// <summary>
        /// Obtener promedio de calificaciones de un destino
        /// GET: api/destination-ratings/average/{destinationId}
        /// </summary>
        [HttpGet("average/{destinationId}")]
        public async Task<DestinationRatingAverageDto> GetAverageAsync(Guid destinationId)
        {
            return await _ratingAppService.GetAverageRatingAsync(destinationId);
        }

        /// <summary>
        /// Listar todas las calificaciones de un destino
        /// GET: api/destination-ratings/destination/{destinationId}
        /// </summary>
        [HttpGet("destination/{destinationId}")]
        public async Task<List<DestinationRatingDto>> GetAllByDestinationAsync(Guid destinationId)
        {
            return await _ratingAppService.GetAllByDestinationAsync(destinationId);
        }

        /// <summary>
        /// Obtener mis calificaciones para un destino
        /// GET: api/destination-ratings/my/{destinationId}
        /// </summary>
        [HttpGet("my/{destinationId}")]
        public async Task<List<DestinationRatingDto>> GetMyRatingsAsync(Guid destinationId)
        {
            return await _ratingAppService.GetMyRatingsAsync(destinationId);
        }
    }
}
