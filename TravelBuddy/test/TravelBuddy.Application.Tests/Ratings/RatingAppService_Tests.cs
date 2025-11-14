using Shouldly;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TravelBuddy.Application.Contracts.Ratings;
using TravelBuddy.Ratings;
using Volo.Abp.Authorization;
using Volo.Abp.Security.Claims;
using Xunit;

namespace TravelBuddy.Application.Tests.Ratings
{
    public class RatingAppService_Tests : TravelBuddyApplicationTestBase<TravelBuddyApplicationTestModule>
    {
        private readonly IRatingAppService _ratingAppService;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

        public RatingAppService_Tests()
        {
            _ratingAppService = GetRequiredService<IRatingAppService>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
        }

        [Fact]
        public async Task CreateAsync_Should_Associate_Rating_With_Current_User()
        {
            await Task.Delay(2000);

            // Arrange
            var testUserId = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");

            await WithUnitOfWorkAsync(async () =>
            {
                // Simular un usuario autenticado
                var claims = new[]
                {
                    new Claim(AbpClaimTypes.UserId, testUserId.ToString()),
                    new Claim(AbpClaimTypes.UserName, "test_user")
                };

                var identity = new ClaimsIdentity(claims, "TestAuthType");
                var principal = new ClaimsPrincipal(identity);

                using (_currentPrincipalAccessor.Change(principal))
                {
                    var input = new CreateUpdateRatingDto
                    {
                        DestinationId = Guid.NewGuid(),
                        Stars = 5,
                        Comment = "Excelente"
                    };

                    // Act
                    var result = await _ratingAppService.CreateAsync(input);

                    // Assert
                    result.ShouldNotBeNull();
                    result.Stars.ShouldBe(5);
                    result.Comment.ShouldBe("Excelente");
                    result.DestinationId.ShouldBe(input.DestinationId);
                    result.UserId.ShouldBe(testUserId);
                }
            });
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_If_Not_Authenticated()
        {
            // Arrange
            var input = new CreateUpdateRatingDto
            {
                DestinationId = Guid.NewGuid(),
                Stars = 4
            };

            // Act & Assert
            await Should.ThrowAsync<AbpAuthorizationException>(async () =>
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    // No configuramos ningún usuario - no autenticado
                    await _ratingAppService.CreateAsync(input);
                });
            });
        }
    }
}