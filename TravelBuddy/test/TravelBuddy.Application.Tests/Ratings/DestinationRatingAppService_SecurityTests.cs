using System;
using System.Security.Claims;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using TravelBuddy.Application.Ratings;
using TravelBuddy.Ratings;
using TravelBuddy.Ratings.Dtos;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Xunit;

namespace TravelBuddy.Tests.Ratings
{
    /// <summary>
    /// Pruebas de seguridad para DestinationRatingAppService
    /// Valida: autorización requerida, comportamiento sin autenticación
    /// </summary>
    public abstract class DestinationRatingAppService_SecurityTests<TStartupModule> 
        : TravelBuddyApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly DestinationRatingAppService _appService;

        protected DestinationRatingAppService_SecurityTests()
        {
            _appService = GetRequiredService<DestinationRatingAppService>();
        }

        [Fact]
        public async Task CreateAsync_DeberiaFallar_SiUsuarioNoAutenticado()
        {
            // Arrange: Simular usuario no autenticado
            var mockCurrentUser = new FakeCurrentUser(isAuthenticated: false);
            var appServiceWithoutAuth = new DestinationRatingAppService(
                GetRequiredService<IRepository<DestinationRating, Guid>>(),
                GetRequiredService<IIdentityUserRepository>(),
                mockCurrentUser,
                GetRequiredService<IDataFilter>()
            );

            var input = new CreateDestinationRatingDto
            {
                DestinationId = Guid.NewGuid(),
                Score = 5,
                Comment = "Test"
            };

            // Act & Assert
            await Should.ThrowAsync<AbpAuthorizationException>(
                async () => await appServiceWithoutAuth.CreateAsync(input));
        }

        [Fact]
        public async Task CreateAsync_DeberiaExitir_ConUsuarioAutenticado()
        {
            // Arrange
            var currentUser = GetRequiredService<ICurrentUser>();
            currentUser.IsAuthenticated.ShouldBeTrue("El test debe ejecutarse con usuario autenticado");

            var input = new CreateDestinationRatingDto
            {
                DestinationId = Guid.NewGuid(),
                Score = 5,
                Comment = "Usuario autenticado"
            };

            // Act
            var result = await _appService.CreateAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.UserId.ShouldBe(currentUser.GetId());
        }

        /// <summary>
        /// Implementación falsa de ICurrentUser para pruebas
        /// </summary>
        private class FakeCurrentUser : ICurrentUser
        {
            private readonly bool _isAuthenticated;

            public FakeCurrentUser(bool isAuthenticated)
            {
                _isAuthenticated = isAuthenticated;
            }

            public bool IsAuthenticated => _isAuthenticated;
            public Guid? Id => _isAuthenticated ? Guid.NewGuid() : null;
            public string? UserName => _isAuthenticated ? "testuser" : null;
            public string? Name => null;
            public string? SurName => null;
            public string? PhoneNumber => null;
            public bool PhoneNumberVerified => false;
            public string? Email => null;
            public bool EmailVerified => false;
            public Guid? TenantId => null;
            public string[] Roles => Array.Empty<string>();

            public Claim? FindClaim(string claimType) => null;
            public Claim[] FindClaims(string claimType) => Array.Empty<Claim>();
            public Claim[] GetAllClaims() => Array.Empty<Claim>();
            public bool IsInRole(string roleName) => false;
        }
    }
}