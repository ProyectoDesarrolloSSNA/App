using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Shouldly;
using TravelBuddy.Destinos;
using TravelBuddy.Ratings;
using TravelBuddy.Ratings.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Xunit;

namespace TravelBuddy.Application.Tests.Ratings
{
    public class DestinationRatingAppService_Tests : TravelBuddyApplicationTestBase<TravelBuddyApplicationTestModule>
    {
        private readonly IDestinationRatingAppService _ratingAppService;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;
        private readonly IIdentityUserRepository _userRepository;

        public DestinationRatingAppService_Tests()
        {
            _ratingAppService = GetRequiredService<IDestinationRatingAppService>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
            _userRepository = GetRequiredService<IIdentityUserRepository>();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateRating_ForAuthenticatedUser()
        {
            // Arrange
            var testUserId = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            var destinationId = Guid.NewGuid();

            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
                {
                    var input = new CreateDestinationRatingDto
                    {
                        DestinationId = destinationId,
                        Score = 5,
                        Comment = "Excelente destino"
                    };

                    // Act
                    var result = await _ratingAppService.CreateAsync(input);

                    // Assert
                    result.ShouldNotBeNull();
                    result.Score.ShouldBe(5);
                    result.Comment.ShouldBe("Excelente destino");
                    result.UserId.ShouldBe(testUserId);
                }
            });
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateOwnRating()
        {
            // Arrange
            var testUserId = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            var destinationId = Guid.NewGuid();
            Guid ratingId = Guid.Empty;

            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
                {
                    // Crear calificación
                    var createInput = new CreateDestinationRatingDto
                    {
                        DestinationId = destinationId,
                        Score = 3,
                        Comment = "Regular"
                    };
                    var created = await _ratingAppService.CreateAsync(createInput);
                    ratingId = created.Id;

                    // Actualizar
                    var updateInput = new UpdateDestinationRatingDto
                    {
                        Score = 5,
                        Comment = "Actualizado: Excelente!"
                    };

                    // Act
                    var result = await _ratingAppService.UpdateAsync(ratingId, updateInput);

                    // Assert
                    result.Score.ShouldBe(5);
                    result.Comment.ShouldBe("Actualizado: Excelente!");
                }
            });
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteOwnRating()
        {
            // Arrange
            var testUserId = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            var destinationId = Guid.NewGuid();
            Guid ratingId = Guid.Empty;

            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
                {
                    // Crear calificación
                    var createInput = new CreateDestinationRatingDto
                    {
                        DestinationId = destinationId,
                        Score = 4,
                        Comment = "Bueno"
                    };
                    var created = await _ratingAppService.CreateAsync(createInput);
                    ratingId = created.Id;

                    // Act
                    await _ratingAppService.DeleteAsync(ratingId);

                    // Assert
                    var ratings = await _ratingAppService.GetAllByDestinationAsync(destinationId);
                    ratings.ShouldBeEmpty();
                }
            });
        }

        [Fact]
        public async Task GetAverageRatingAsync_ShouldCalculateCorrectAverage()
        {
            // Arrange
            var destinationId = Guid.NewGuid();
            var user1Id = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            var user2Id = Guid.Parse("3e701e62-0953-4dd3-910b-dc6cc93ccb0d");

            await WithUnitOfWorkAsync(async () =>
            {
                // Usuario 1 califica con 5
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(user1Id)))
                {
                    await _ratingAppService.CreateAsync(new CreateDestinationRatingDto
                    {
                        DestinationId = destinationId,
                        Score = 5,
                        Comment = "Excelente"
                    });
                }

                // Usuario 2 califica con 3
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(user2Id)))
                {
                    await _ratingAppService.CreateAsync(new CreateDestinationRatingDto
                    {
                        DestinationId = destinationId,
                        Score = 3,
                        Comment = "Regular"
                    });
                }

                // Resetear el contexto de usuario antes de obtener el promedio
                using (_currentPrincipalAccessor.Change(null))
                {
                    // Act
                    var result = await _ratingAppService.GetAverageRatingAsync(destinationId);

                    // Assert
                    result.AverageScore.ShouldBe(4.0);
                    result.TotalRatings.ShouldBe(2);
                }
            });
        }

        [Fact]
        public async Task GetAllByDestinationAsync_ShouldReturnAllRatings()
        {
            // Arrange
            var destinationId = Guid.NewGuid();
            var user1Id = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");

            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(user1Id)))
                {
                    await _ratingAppService.CreateAsync(new CreateDestinationRatingDto
                    {
                        DestinationId = destinationId,
                        Score = 5,
                        Comment = "Excelente"
                    });

                    // Act
                    var result = await _ratingAppService.GetAllByDestinationAsync(destinationId);

                    // Assert
                    result.ShouldNotBeEmpty();
                    result.Count.ShouldBe(1);
                    result.First().Comment.ShouldBe("Excelente");
                }
            });
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenNotAuthenticated()
        {
            // Arrange
            var input = new CreateDestinationRatingDto
            {
                DestinationId = Guid.NewGuid(),
                Score = 5
            };

            // Act & Assert
            await Should.ThrowAsync<AbpAuthorizationException>(async () =>
            {
                await _ratingAppService.CreateAsync(input);
            });
        }

        private ClaimsPrincipal CreateTestPrincipal(Guid userId)
        {
            var claims = new[]
            {
                new Claim(AbpClaimTypes.UserId, userId.ToString()),
                new Claim(AbpClaimTypes.UserName, $"test_user_{userId}")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            return new ClaimsPrincipal(identity);
        }
    }
}
