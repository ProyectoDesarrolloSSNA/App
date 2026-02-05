using System;
using System.Threading.Tasks;
using Shouldly;
using TravelBuddy.Users;
using Volo.Abp.Identity;
using Xunit;

namespace TravelBuddy.Application.Tests.Users
{
    public class PublicProfileAppService_Tests : TravelBuddyApplicationTestBase<TravelBuddyApplicationTestModule>
    {
        private readonly IPublicProfileAppService _publicProfileAppService;
        private readonly IIdentityUserRepository _userRepository;

        public PublicProfileAppService_Tests()
        {
            _publicProfileAppService = GetRequiredService<IPublicProfileAppService>();
            _userRepository = GetRequiredService<IIdentityUserRepository>();
        }

        [Fact]
        public async Task GetPublicProfileAsync_ShouldReturnUserProfile()
        {
            // Arrange
            var adminUser = await _userRepository.FindByNormalizedUserNameAsync("ADMIN");
            adminUser.ShouldNotBeNull();

            // Act
            var profile = await _publicProfileAppService.GetPublicProfileAsync(adminUser.Id);

            // Assert
            profile.ShouldNotBeNull();
            profile.Id.ShouldBe(adminUser.Id);
            profile.UserName.ShouldBe(adminUser.UserName);
        }

        [Fact]
        public async Task GetPublicProfileByUserNameAsync_ShouldReturnUserProfile()
        {
            // Arrange
            var userName = "admin";

            // Act
            var profile = await _publicProfileAppService.GetPublicProfileByUserNameAsync(userName);

            // Assert
            profile.ShouldNotBeNull();
            profile.UserName.ShouldBe(userName);
        }

        [Fact]
        public async Task GetPublicProfileAsync_ShouldThrowException_ForInvalidUserId()
        {
            // Arrange
            var invalidUserId = Guid.NewGuid();

            // Act & Assert
            await Should.ThrowAsync<Volo.Abp.Domain.Entities.EntityNotFoundException>(
                async () => await _publicProfileAppService.GetPublicProfileAsync(invalidUserId)
            );
        }

        [Fact]
        public async Task GetPublicProfileByUserNameAsync_ShouldThrowException_ForInvalidUserName()
        {
            // Arrange
            var invalidUserName = "usuario_inexistente";

            // Act & Assert
            await Should.ThrowAsync<Volo.Abp.UserFriendlyException>(
                async () => await _publicProfileAppService.GetPublicProfileByUserNameAsync(invalidUserName)
            );
        }
    }
}