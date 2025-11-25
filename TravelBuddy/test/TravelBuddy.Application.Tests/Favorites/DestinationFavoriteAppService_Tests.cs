using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Shouldly;
using TravelBuddy.Destinos;
using TravelBuddy.Favorites;
using TravelBuddy.Favorites.Dtos;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Security.Claims;
using Xunit;

namespace TravelBuddy.Application.Tests.Favorites
{
    public class DestinationFavoriteAppService_Tests : TravelBuddyApplicationTestBase<TravelBuddyApplicationTestModule>
    {
        private readonly IDestinationFavoriteAppService _favoriteAppService;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;
        private readonly IRepository<Destino, Guid> _destinationRepository;
        private readonly IRepository<DestinationFavorite, Guid> _favoriteRepository;

        public DestinationFavoriteAppService_Tests()
        {
            _favoriteAppService = GetRequiredService<IDestinationFavoriteAppService>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
            _destinationRepository = GetRequiredService<IRepository<Destino, Guid>>();
            _favoriteRepository = GetRequiredService<IRepository<DestinationFavorite, Guid>>();
        }

        [Fact]
        public async Task AddToFavoritesAsync_ShouldAddDestinationToFavorites()
        {
            // Arrange
            var testUserId = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            Guid destinationId = Guid.Empty;

            // Crear un destino primero
            await WithUnitOfWorkAsync(async () =>
            {
                var destination = new Destino(
                    Guid.NewGuid(),
                    "París",
                    "Francia",
                    "La ciudad del amor"
                );
                await _destinationRepository.InsertAsync(destination, autoSave: true);
                destinationId = destination.Id;
            });

            // Agregar a favoritos
            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
                {
                    var input = new AddDestinationToFavoritesDto
                    {
                        DestinationId = destinationId
                    };

                    // Act
                    var result = await _favoriteAppService.AddToFavoritesAsync(input);

                    // Assert
                    result.ShouldNotBeNull();
                    result.DestinationId.ShouldBe(destinationId);
                    result.UserId.ShouldBe(testUserId);
                    result.DestinationName.ShouldBe("París");
                    result.DestinationCountry.ShouldBe("Francia");
                }
            });
        }

        [Fact]
        public async Task AddToFavoritesAsync_ShouldThrowException_WhenDestinationDoesNotExist()
        {
            // Arrange
            var testUserId = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            var nonExistentDestinationId = Guid.NewGuid();

            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
                {
                    var input = new AddDestinationToFavoritesDto
                    {
                        DestinationId = nonExistentDestinationId
                    };

                    // Act & Assert
                    await Should.ThrowAsync<UserFriendlyException>(async () =>
                    {
                        await _favoriteAppService.AddToFavoritesAsync(input);
                    });
                }
            });
        }

        [Fact]
        public async Task AddToFavoritesAsync_ShouldThrowException_WhenAlreadyInFavorites()
        {
            // Arrange
            var testUserId = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            Guid destinationId = Guid.Empty;

            // Crear destino
            await WithUnitOfWorkAsync(async () =>
            {
                var destination = new Destino(
                    Guid.NewGuid(),
                    "Roma",
                    "Italia",
                    "Ciudad eterna"
                );
                await _destinationRepository.InsertAsync(destination, autoSave: true);
                destinationId = destination.Id;
            });

            // Agregar a favoritos por primera vez
            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
                {
                    var input = new AddDestinationToFavoritesDto
                    {
                        DestinationId = destinationId
                    };
                    await _favoriteAppService.AddToFavoritesAsync(input);
                }
            });

            // Intentar agregar de nuevo
            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
                {
                    var input = new AddDestinationToFavoritesDto
                    {
                        DestinationId = destinationId
                    };

                    // Act & Assert
                    await Should.ThrowAsync<UserFriendlyException>(async () =>
                    {
                        await _favoriteAppService.AddToFavoritesAsync(input);
                    });
                }
            });
        }

        [Fact]
        public async Task RemoveFromFavoritesAsync_ShouldRemoveDestinationFromFavorites()
        {
            // Arrange
            var testUserId = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            Guid destinationId = Guid.Empty;

            // Crear destino
            await WithUnitOfWorkAsync(async () =>
            {
                var destination = new Destino(
                    Guid.NewGuid(),
                    "Barcelona",
                    "España",
                    "Ciudad hermosa"
                );
                await _destinationRepository.InsertAsync(destination, autoSave: true);
                destinationId = destination.Id;
            });

            // Agregar a favoritos
            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
                {
                    var input = new AddDestinationToFavoritesDto
                    {
                        DestinationId = destinationId
                    };
                    await _favoriteAppService.AddToFavoritesAsync(input);
                }
            });

            // Remover de favoritos
            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
                {
                    // Act
                    await _favoriteAppService.RemoveFromFavoritesAsync(destinationId);

                    // Assert
                    var favorites = await _favoriteAppService.GetMyFavoritesAsync();
                    favorites.ShouldBeEmpty();
                }
            });
        }

        [Fact]
        public async Task RemoveFromFavoritesAsync_ShouldThrowException_WhenNotInFavorites()
        {
            // Arrange
            var testUserId = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            Guid destinationId = Guid.Empty;

            // Crear destino
            await WithUnitOfWorkAsync(async () =>
            {
                var destination = new Destino(
                    Guid.NewGuid(),
                    "Londres",
                    "Reino Unido",
                    "Capital británica"
                );
                await _destinationRepository.InsertAsync(destination, autoSave: true);
                destinationId = destination.Id;
            });

            // Intentar remover sin agregarlo primero
            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
                {
                    // Act & Assert
                    await Should.ThrowAsync<UserFriendlyException>(async () =>
                    {
                        await _favoriteAppService.RemoveFromFavoritesAsync(destinationId);
                    });
                }
            });
        }

        [Fact]
        public async Task GetMyFavoritesAsync_ShouldReturnUserFavorites()
        {
            // Arrange
            var testUserId = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            Guid destination1Id = Guid.Empty;
            Guid destination2Id = Guid.Empty;

            // Crear dos destinos
            await WithUnitOfWorkAsync(async () =>
            {
                var destination1 = new Destino(
                    Guid.NewGuid(),
                    "Tokyo",
                    "Japón",
                    "Capital japonesa"
                );
                var destination2 = new Destino(
                    Guid.NewGuid(),
                    "Nueva York",
                    "Estados Unidos",
                    "La gran manzana"
                );
                await _destinationRepository.InsertAsync(destination1, autoSave: true);
                await _destinationRepository.InsertAsync(destination2, autoSave: true);
                destination1Id = destination1.Id;
                destination2Id = destination2.Id;
            });

            // Agregar ambos a favoritos
            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
                {
                    await _favoriteAppService.AddToFavoritesAsync(new AddDestinationToFavoritesDto
                    {
                        DestinationId = destination1Id
                    });
                    await _favoriteAppService.AddToFavoritesAsync(new AddDestinationToFavoritesDto
                    {
                        DestinationId = destination2Id
                    });
                }
            });

            // Obtener favoritos
            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
                {
                    // Act
                    var result = await _favoriteAppService.GetMyFavoritesAsync();

                    // Assert
                    result.ShouldNotBeEmpty();
                    result.Count.ShouldBe(2);
                    result.ShouldContain(f => f.DestinationName == "Tokyo");
                    result.ShouldContain(f => f.DestinationName == "Nueva York");
                }
            });
        }

        [Fact]
        public async Task GetMyFavoritesAsync_ShouldReturnEmpty_WhenNoFavorites()
        {
            // Arrange
            var testUserId = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");

            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
                {
                    // Act
                    var result = await _favoriteAppService.GetMyFavoritesAsync();

                    // Assert
                    result.ShouldBeEmpty();
                }
            });
        }

        [Fact]
        public async Task IsInFavoritesAsync_ShouldReturnTrue_WhenDestinationIsInFavorites()
        {
            // Arrange
            var testUserId = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            Guid destinationId = Guid.Empty;

            // Crear destino
            await WithUnitOfWorkAsync(async () =>
            {
                var destination = new Destino(
                    Guid.NewGuid(),
                    "Berlín",
                    "Alemania",
                    "Capital alemana"
                );
                await _destinationRepository.InsertAsync(destination, autoSave: true);
                destinationId = destination.Id;
            });

            // Agregar a favoritos
            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
                {
                    await _favoriteAppService.AddToFavoritesAsync(new AddDestinationToFavoritesDto
                    {
                        DestinationId = destinationId
                    });
                }
            });

            // Verificar
            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
                {
                    // Act
                    var result = await _favoriteAppService.IsInFavoritesAsync(destinationId);

                    // Assert
                    result.ShouldBeTrue();
                }
            });
        }

        [Fact]
        public async Task IsInFavoritesAsync_ShouldReturnFalse_WhenDestinationIsNotInFavorites()
        {
            // Arrange
            var testUserId = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            Guid destinationId = Guid.Empty;

            // Crear destino
            await WithUnitOfWorkAsync(async () =>
            {
                var destination = new Destino(
                    Guid.NewGuid(),
                    "Amsterdam",
                    "Países Bajos",
                    "Ciudad de canales"
                );
                await _destinationRepository.InsertAsync(destination, autoSave: true);
                destinationId = destination.Id;
            });

            // Verificar sin agregar a favoritos
            await WithUnitOfWorkAsync(async () =>
            {
                using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
                {
                    // Act
                    var result = await _favoriteAppService.IsInFavoritesAsync(destinationId);

                    // Assert
                    result.ShouldBeFalse();
                }
            });
        }

        [Fact]
        public async Task AddToFavoritesAsync_ShouldThrowException_WhenNotAuthenticated()
        {
            // Arrange
            var input = new AddDestinationToFavoritesDto
            {
                DestinationId = Guid.NewGuid()
            };

            // Act & Assert
            await Should.ThrowAsync<AbpAuthorizationException>(async () =>
            {
                await _favoriteAppService.AddToFavoritesAsync(input);
            });
        }

        [Fact]
        public async Task GetMyFavoritesAsync_ShouldThrowException_WhenNotAuthenticated()
        {
            // Act & Assert
            await Should.ThrowAsync<AbpAuthorizationException>(async () =>
            {
                await _favoriteAppService.GetMyFavoritesAsync();
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
