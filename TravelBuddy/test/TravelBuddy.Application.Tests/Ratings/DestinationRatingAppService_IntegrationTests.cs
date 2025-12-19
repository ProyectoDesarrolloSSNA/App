using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TravelBuddy.Application.Ratings;
using TravelBuddy.EntityFrameworkCore;
using TravelBuddy.Ratings;
using TravelBuddy.Ratings.Dtos;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Xunit;

namespace TravelBuddy.Tests.Ratings
{
    /// <summary>
    /// Pruebas de integración para DestinationRatingAppService
    /// Valida: filtros por usuario, persistencia en BD, query filters de EF Core
    /// </summary>
    public abstract class DestinationRatingAppService_IntegrationTests<TStartupModule> 
        : TravelBuddyApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly DestinationRatingAppService _appService;
        private readonly TravelBuddyDbContext _dbContext;

        protected DestinationRatingAppService_IntegrationTests()
        {
            _appService = GetRequiredService<DestinationRatingAppService>();
            _dbContext = GetRequiredService<TravelBuddyDbContext>();
        }

        #region Pruebas de Creación y Persistencia

        [Fact]
        public async Task CreateAsync_DeberiaPersistir_CalificacionEnBaseDeDatos()
        {
            // Arrange
            var destinationId = Guid.NewGuid();
            var input = new CreateDestinationRatingDto
            {
                DestinationId = destinationId,
                Score = 5,
                Comment = "Excelente destino"
            };

            // Act
            var result = await _appService.CreateAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            result.DestinationId.ShouldBe(destinationId);
            result.Score.ShouldBe(5);
            result.Comment.ShouldBe("Excelente destino");

            // Verificar en base de datos
            var savedRating = await _dbContext.DestinationRatings.FindAsync(result.Id);
            savedRating.ShouldNotBeNull();
            savedRating.Score.ShouldBe(5);
        }

        [Fact]
        public async Task CreateAsync_DeberiaAsignar_UsuarioActualCorrectamente()
        {
            // Arrange
            var currentUser = GetRequiredService<ICurrentUser>();
            var input = new CreateDestinationRatingDto
            {
                DestinationId = Guid.NewGuid(),
                Score = 4,
                Comment = null
            };

            // Act
            var result = await _appService.CreateAsync(input);

            // Assert
            result.UserId.ShouldBe(currentUser.GetId());
        }

        [Fact]
        public async Task CreateAsync_DeberiaPermitir_ComentarioOpcional()
        {
            // Arrange
            var input = new CreateDestinationRatingDto
            {
                DestinationId = Guid.NewGuid(),
                Score = 3,
                Comment = null
            };

            // Act
            var result = await _appService.CreateAsync(input);

            // Assert
            result.Comment.ShouldBeNull();
        }

        #endregion

        #region Pruebas de Filtro por Usuario (Query Filter)

        [Fact]
        public async Task GetMyRatingsAsync_DeberiaDevolverSolo_CalificacionesDelUsuarioActual()
        {
            // Arrange: Crear calificaciones de diferentes usuarios
            var destinationId = Guid.NewGuid();
            var currentUserId = GetRequiredService<ICurrentUser>().GetId();
            var otherUserId = Guid.NewGuid();

            // Calificación del usuario actual
            var myRating = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: destinationId,
                score: 5,
                comment: "Mi calificación",
                userId: currentUserId
            );

            // Calificación de otro usuario (NO debería aparecer por el query filter)
            var otherRating = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: destinationId,
                score: 3,
                comment: "Calificación de otro usuario",
                userId: otherUserId
            );

            await _dbContext.DestinationRatings.AddRangeAsync(myRating, otherRating);
            await _dbContext.SaveChangesAsync();

            // Act
            var results = await _appService.GetMyRatingsAsync(destinationId);

            // Assert
            results.ShouldNotBeEmpty();
            results.Count.ShouldBe(1, "Solo debe devolver la calificación del usuario actual");
            results.First().Comment.ShouldBe("Mi calificación");
            results.First().UserId.ShouldBe(currentUserId);
        }

        [Fact]
        public async Task GetMyRatingsAsync_DeberiaDevolverVacio_SiUsuarioNoTieneCalificaciones()
        {
            // Arrange
            var destinationId = Guid.NewGuid();

            // Act
            var results = await _appService.GetMyRatingsAsync(destinationId);

            // Assert
            results.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetMyRatingsAsync_DeberiaDevolverMultiples_CalificacionesDelMismoUsuario()
        {
            // Arrange
            var destinationId = Guid.NewGuid();
            var currentUserId = GetRequiredService<ICurrentUser>().GetId();

            var rating1 = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: destinationId,
                score: 4,
                comment: "Primera calificación",
                userId: currentUserId
            );

            var rating2 = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: destinationId,
                score: 5,
                comment: "Segunda calificación",
                userId: currentUserId
            );

            await _dbContext.DestinationRatings.AddRangeAsync(rating1, rating2);
            await _dbContext.SaveChangesAsync();

            // Act
            var results = await _appService.GetMyRatingsAsync(destinationId);

            // Assert
            results.Count.ShouldBe(2);
            results.All(r => r.UserId == currentUserId).ShouldBeTrue();
        }

        #endregion

        #region Pruebas de Validación

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        [InlineData(-1)]
        public async Task CreateAsync_DeberiaFallar_ConPuntuacionInvalida(int invalidScore)
        {
            // Arrange
            var input = new CreateDestinationRatingDto
            {
                DestinationId = Guid.NewGuid(),
                Score = invalidScore,
                Comment = null
            };

            // Act & Assert
            await Should.ThrowAsync<ArgumentOutOfRangeException>(
                async () => await _appService.CreateAsync(input));
        }

        #endregion

        #region Prevención de Duplicados (Regla de Negocio)

        [Fact]
        public async Task Usuario_DeberiaPoderCrear_MultiplesCalificacionesParaMismoDestino()
        {
            // Arrange: Escenario actual permite múltiples calificaciones
            var destinationId = Guid.NewGuid();
            var input1 = new CreateDestinationRatingDto
            {
                DestinationId = destinationId,
                Score = 4,
                Comment = "Primera vez"
            };

            var input2 = new CreateDestinationRatingDto
            {
                DestinationId = destinationId,
                Score = 5,
                Comment = "Mejoró mucho"
            };

            // Act
            var result1 = await _appService.CreateAsync(input1);
            var result2 = await _appService.CreateAsync(input2);

            // Assert
            result1.Id.ShouldNotBe(result2.Id);
            
            var allRatings = await _appService.GetMyRatingsAsync(destinationId);
            allRatings.Count.ShouldBe(2);
        }

        #endregion
    }
}