using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TravelBuddy.Users;
using Volo.Abp.AspNetCore.Mvc;

namespace TravelBuddy.HttpApi.Controllers.Users
{
    [Route("api/users/public-profile")]
    [ApiController]
    public class PublicProfileController : AbpControllerBase
    {
        private readonly IPublicProfileAppService _publicProfileAppService;

        public PublicProfileController(IPublicProfileAppService publicProfileAppService)
        {
            _publicProfileAppService = publicProfileAppService;
        }

        /// <summary>
        /// Obtiene el perfil público de un usuario por ID
        /// GET: api/users/public-profile/{userId}
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<PublicProfileDto> GetByIdAsync(Guid userId)
        {
            return await _publicProfileAppService.GetPublicProfileAsync(userId);
        }

        /// <summary>
        /// Obtiene el perfil público de un usuario por nombre de usuario
        /// GET: api/users/public-profile/by-username/{userName}
        /// </summary>
        [HttpGet("by-username/{userName}")]
        public async Task<PublicProfileDto> GetByUserNameAsync(string userName)
        {
            return await _publicProfileAppService.GetPublicProfileByUserNameAsync(userName);
        }
    }
}
