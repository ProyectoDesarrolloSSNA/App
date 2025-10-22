using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelBuddy.Ratings;
using TravelBuddy.Ratings.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace TravelBuddy.Application.Ratings
{
    [Authorize]
    public class DestinationRatingAppService : ApplicationService
    {
        private readonly IRepository<DestinationRating, Guid> _repo;
        private readonly ICurrentUser _currentUser;

        public DestinationRatingAppService(IRepository<DestinationRating, Guid> repo, ICurrentUser currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        public async Task<DestinationRatingDto> CreateAsync(CreateDestinationRatingDto input)
        {
            if (!_currentUser.IsAuthenticated) throw new AbpAuthorizationException();

            var entity = new DestinationRating(
                id: GuidGenerator.Create(),
                destinationId: input.DestinationId,
                score: input.Score,
                comment: input.Comment,
                userId: _currentUser.GetId()
            );

            await _repo.InsertAsync(entity, autoSave: true);
            return ObjectMapper.Map<DestinationRating, DestinationRatingDto>(entity);
        }

        public async Task<List<DestinationRatingDto>> GetMyRatingsAsync(Guid destinationId)
        {
            var list = await _repo.GetListAsync(x => x.DestinationId == destinationId);
            return ObjectMapper.Map<List<DestinationRating>, List<DestinationRatingDto>>(list);
        }
    }
}
