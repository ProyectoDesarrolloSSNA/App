using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelBuddy.Ratings;
using TravelBuddy.Ratings.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace TravelBuddy.Application.Ratings
{
    public class DestinationRatingAppService : ApplicationService, IDestinationRatingAppService
    {
        private readonly IRepository<DestinationRating, Guid> _repo;
        private readonly IIdentityUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IDataFilter _dataFilter;

        public DestinationRatingAppService(
            IRepository<DestinationRating, Guid> repo,
            IIdentityUserRepository userRepository,
            ICurrentUser currentUser,
            IDataFilter dataFilter)
        {
            _repo = repo;
            _userRepository = userRepository;
            _currentUser = currentUser;
            _dataFilter = dataFilter;
        }

        [Authorize]
        public async Task<DestinationRatingDto> CreateAsync(CreateDestinationRatingDto input)
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException();

            // Verificar si el usuario ya qualificó este destino
            var existingRating = await _repo.FirstOrDefaultAsync(x =>
                x.DestinationId == input.DestinationId &&
                x.UserId == _currentUser.GetId());

            if (existingRating != null)
            {
                throw new UserFriendlyException("Ya has calificado este destino. Usa la opción de editar.");
            }

            var entity = new DestinationRating(
                id: GuidGenerator.Create(),
                destinationId: input.DestinationId,
                score: input.Score,
                comment: input.Comment,
                userId: _currentUser.GetId()
            );

            await _repo.InsertAsync(entity, autoSave: true);
            
            return await MapToDto(entity);
        }

        [Authorize]
        public async Task<DestinationRatingDto> UpdateAsync(Guid id, UpdateDestinationRatingDto input)
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException();

            var rating = await _repo.GetAsync(id);

            // Verificar que el usuario solo pueda editar sus propias calificaciones
            if (rating.UserId != _currentUser.GetId())
            {
                throw new AbpAuthorizationException("Solo puedes editar tus propias calificaciones.");
            }

            rating.Update(input.Comment, input.Score);
            await _repo.UpdateAsync(rating, autoSave: true);

            return await MapToDto(rating);
        }

        [Authorize]
        public async Task DeleteAsync(Guid id)
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException();

            var rating = await _repo.GetAsync(id);

            // Verificar que el usuario solo pueda eliminar sus propias calificaciones
            if (rating.UserId != _currentUser.GetId())
            {
                throw new AbpAuthorizationException("Solo puedes eliminar tus propias calificaciones.");
            }

            await _repo.DeleteAsync(rating, autoSave: true);
        }

        public async Task<DestinationRatingAverageDto> GetAverageRatingAsync(Guid destinationId)
        {
            var queryable = await _repo.GetQueryableAsync();
            var ratings = queryable.Where(x => x.DestinationId == destinationId);
            
            var count = await AsyncExecuter.CountAsync(ratings);
            
            if (count == 0)
            {
                return new DestinationRatingAverageDto
                {
                    AverageScore = 0,
                    TotalRatings = 0
                };
            }

            var average = await AsyncExecuter.AverageAsync(ratings, x => x.Score);

            return new DestinationRatingAverageDto
            {
                AverageScore = Math.Round(average, 2),
                TotalRatings = count
            };
        }

        public async Task<List<DestinationRatingDto>> GetAllByDestinationAsync(Guid destinationId)
        {
            var queryable = await _repo.GetQueryableAsync();
            var ratings = await AsyncExecuter.ToListAsync(
                queryable
                    .Where(x => x.DestinationId == destinationId)
                    .OrderByDescending(x => x.CreationTime)
            );

            var result = new List<DestinationRatingDto>();
            
            foreach (var rating in ratings)
            {
                result.Add(await MapToDto(rating));
            }

            return result;
        }

        [Authorize]
        public async Task<List<DestinationRatingDto>> GetMyRatingsAsync(Guid destinationId)
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException();

            var queryable = await _repo.GetQueryableAsync();
            var ratings = await AsyncExecuter.ToListAsync(
                queryable.Where(x => x.DestinationId == destinationId && x.UserId == _currentUser.GetId())
            );

            var result = new List<DestinationRatingDto>();
            
            foreach (var rating in ratings)
            {
                result.Add(await MapToDto(rating));
            }

            return result;
        }

        /// <summary>
        /// Obtiene la calificación del usuario actual para un destino específico (si existe)
        /// </summary>
        [Authorize]
        public async Task<DestinationRatingDto?> GetMyRatingForDestinationAsync(Guid destinationId)
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException();

            var rating = await _repo.FirstOrDefaultAsync(x => 
                x.DestinationId == destinationId && 
                x.UserId == _currentUser.GetId());

            return rating != null ? await MapToDto(rating) : null;
        }

        private async Task<DestinationRatingDto> MapToDto(DestinationRating rating)
        {
            var user = await _userRepository.GetAsync(rating.UserId);
            
            return new DestinationRatingDto
            {
                Id = rating.Id,
                DestinationId = rating.DestinationId,
                UserId = rating.UserId,
                UserName = user.UserName,
                Score = rating.Score,
                Comment = rating.Comment,
                CreationTime = rating.CreationTime
            };
        }
    }
}
