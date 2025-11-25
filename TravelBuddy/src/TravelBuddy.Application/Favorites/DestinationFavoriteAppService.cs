using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelBuddy.Destinos;
using TravelBuddy.Favorites.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace TravelBuddy.Favorites
{
    [Authorize]
    public class DestinationFavoriteAppService : ApplicationService, IDestinationFavoriteAppService
    {
        private readonly IRepository<DestinationFavorite, Guid> _favoriteRepository;
        private readonly IRepository<Destino, Guid> _destinationRepository;
        private readonly ICurrentUser _currentUser;

        public DestinationFavoriteAppService(
            IRepository<DestinationFavorite, Guid> favoriteRepository,
            IRepository<Destino, Guid> destinationRepository,
            ICurrentUser currentUser)
        {
            _favoriteRepository = favoriteRepository;
            _destinationRepository = destinationRepository;
            _currentUser = currentUser;
        }

        public async Task<DestinationFavoriteDto> AddToFavoritesAsync(AddDestinationToFavoritesDto input)
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debe estar autenticado para agregar favoritos.");

            var userId = _currentUser.GetId();

            // Verificar que el destino existe
            var destinationExists = await _destinationRepository.AnyAsync(d => d.Id == input.DestinationId);
            if (!destinationExists)
            {
                throw new UserFriendlyException("El destino especificado no existe.");
            }

            // Verificar si ya está en favoritos
            var existingFavorite = await _favoriteRepository.FirstOrDefaultAsync(f =>
                f.DestinationId == input.DestinationId &&
                f.UserId == userId);

            if (existingFavorite != null)
            {
                throw new UserFriendlyException("Este destino ya está en tus favoritos.");
            }

            // Crear el favorito
            var favorite = new DestinationFavorite(
                id: GuidGenerator.Create(),
                destinationId: input.DestinationId,
                userId: userId
            );

            await _favoriteRepository.InsertAsync(favorite, autoSave: true);

            return await MapToDto(favorite);
        }

        public async Task RemoveFromFavoritesAsync(Guid destinationId)
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debe estar autenticado para eliminar favoritos.");

            var userId = _currentUser.GetId();

            var favorite = await _favoriteRepository.FirstOrDefaultAsync(f =>
                f.DestinationId == destinationId &&
                f.UserId == userId);

            if (favorite == null)
            {
                throw new UserFriendlyException("Este destino no está en tus favoritos.");
            }

            await _favoriteRepository.DeleteAsync(favorite, autoSave: true);
        }

        public async Task<List<DestinationFavoriteDto>> GetMyFavoritesAsync()
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debe estar autenticado para ver favoritos.");

            var userId = _currentUser.GetId();

            var queryable = await _favoriteRepository.GetQueryableAsync();
            var favorites = await AsyncExecuter.ToListAsync(
                queryable
                    .Where(f => f.UserId == userId)
                    .OrderByDescending(f => f.CreationTime)
            );

            var result = new List<DestinationFavoriteDto>();
            foreach (var favorite in favorites)
            {
                result.Add(await MapToDto(favorite));
            }

            return result;
        }

        public async Task<bool> IsInFavoritesAsync(Guid destinationId)
        {
            if (!_currentUser.IsAuthenticated)
                return false;

            var userId = _currentUser.GetId();

            return await _favoriteRepository.AnyAsync(f =>
                f.DestinationId == destinationId &&
                f.UserId == userId);
        }

        private async Task<DestinationFavoriteDto> MapToDto(DestinationFavorite favorite)
        {
            var destination = await _destinationRepository.GetAsync(favorite.DestinationId);

            return new DestinationFavoriteDto
            {
                Id = favorite.Id,
                DestinationId = favorite.DestinationId,
                UserId = favorite.UserId,
                CreationTime = favorite.CreationTime,
                DestinationName = destination.Nombre,
                DestinationCountry = destination.Pais
            };
        }
    }
}
