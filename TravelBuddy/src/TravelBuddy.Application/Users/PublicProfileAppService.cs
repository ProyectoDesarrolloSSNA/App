using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace TravelBuddy.Users
{
    public class PublicProfileAppService : ApplicationService, IPublicProfileAppService
    {
        private readonly IIdentityUserRepository _userRepository;
        private readonly IdentityUserManager _userManager;

        public PublicProfileAppService(
            IIdentityUserRepository userRepository,
            IdentityUserManager userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<PublicProfileDto> GetPublicProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
            
            return MapToPublicProfileDto(user);
        }

        public async Task<PublicProfileDto> GetPublicProfileByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("El nombre de usuario no puede estar vacío", nameof(userName));
            }

            var user = await _userManager.FindByNameAsync(userName);
            
            if (user == null)
            {
                throw new Volo.Abp.UserFriendlyException($"No se encontró el usuario con nombre '{userName}'");
            }

            return MapToPublicProfileDto(user);
        }

        private PublicProfileDto MapToPublicProfileDto(IdentityUser user)
        {
            return new PublicProfileDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                CreationTime = user.CreationTime
            };
        }
    }
}
