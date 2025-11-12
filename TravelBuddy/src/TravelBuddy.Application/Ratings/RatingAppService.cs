using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using TravelBuddy.Ratings;
using TravelBuddy.Application.Contracts.Ratings;
using Volo.Abp.Users;
using Volo.Abp;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace TravelBuddy.Ratings
{
    [Authorize]
    public class RatingAppService : CrudAppService<
        Rating,
        RatingDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateRatingDto>, IRatingAppService
    {
        private readonly ICurrentUser _currentUser;

        public RatingAppService(IRepository<Rating, Guid> repository, ICurrentUser currentUser)
            : base(repository)
        {
            _currentUser = currentUser;
        }

        public override async Task<RatingDto> CreateAsync(CreateUpdateRatingDto input)
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException();

            var entity = ObjectMapper.Map<CreateUpdateRatingDto, Rating>(input);
            entity.UserId = _currentUser.GetId();
            await Repository.InsertAsync(entity);
            return ObjectMapper.Map<Rating, RatingDto>(entity);
        }
    }

    public interface IRatingAppService : ICrudAppService<
        RatingDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateRatingDto>
    {
    }
}
