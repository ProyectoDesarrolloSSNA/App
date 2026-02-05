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
        public async Task Usuario_NoPuedeCrear_MultiplesCalificacionesParaMismoDestino()
        {
            // Arrange: La regla de negocio NO permite múltiples calificaciones del mismo usuario para el mismo destino
            var destinationId = Guid.NewGuid();
            var input1 = new CreateDestinationRatingDto
            {
                DestinationId = destinationId,
                Score = 4,
                Comment = "Primera calificación"
            };

            // Act: Crear la primera calificación (debe funcionar)
            var result1 = await _appService.CreateAsync(input1);
            result1.ShouldNotBeNull();

            // Intentar crear una segunda calificación para el mismo destino (debe fallar)
            var input2 = new CreateDestinationRatingDto
            {
                DestinationId = destinationId,
                Score = 5,
                Comment = "Intento de segunda calificación"
            };

            // Assert: Debe lanzar excepción UserFriendlyException
            var exception = await Should.ThrowAsync<Volo.Abp.UserFriendlyException>(
                async () => await _appService.CreateAsync(input2));
            
            exception.Message.ShouldContain("Ya has calificado este destino");
            
            // Verificar que solo existe una calificación
            var allRatings = await _appService.GetMyRatingsAsync(destinationId);
            allRatings.Count.ShouldBe(1, "Solo debe existir una calificación del usuario para este destino");
            allRatings.First().Comment.ShouldBe("Primera calificación");
        }

        [Fact]
        public async Task Usuario_PuedeActualizar_CalificacionExistente()
        {
            // Arrange: Crear una calificación inicial
            var destinationId = Guid.NewGuid();
            var createInput = new CreateDestinationRatingDto
            {
                DestinationId = destinationId,
                Score = 3,
                Comment = "Calificación inicial"
            };

            var created = await _appService.CreateAsync(createInput);

            // Act: Actualizar la calificación existente
            var updateInput = new UpdateDestinationRatingDto
            {
                Score = 5,
                Comment = "Calificación actualizada - mejoró mucho"
            };

            var updated = await _appService.UpdateAsync(created.Id, updateInput);

            // Assert
            updated.Id.ShouldBe(created.Id);
            updated.Score.ShouldBe(5);
            updated.Comment.ShouldBe("Calificación actualizada - mejoró mucho");
            
            // Verificar que sigue siendo solo una calificación
            var allRatings = await _appService.GetMyRatingsAsync(destinationId);
            allRatings.Count.ShouldBe(1);
        }

        #endregion
    }
}