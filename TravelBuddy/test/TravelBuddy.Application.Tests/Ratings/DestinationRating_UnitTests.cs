using System;
using Shouldly;
using TravelBuddy.Ratings;
using Xunit;

namespace TravelBuddy.Tests.Ratings
{
    /// <summary>
    /// Pruebas unitarias para la lógica de negocio de DestinationRating
    /// Valida: puntuación válida, duplicados conceptuales, comentarios opcionales
    /// </summary>
    public class DestinationRating_UnitTests
    {
        #region Validación de Puntuación

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Constructor_DeberiaAceptar_PuntuacionesValidas(int score)
        {
            // Arrange & Act
            var rating = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: Guid.NewGuid(),
                score: score,
                comment: "Comentario de prueba",
                userId: Guid.NewGuid()
            );

            // Assert
            rating.Score.ShouldBe(score);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(6)]
        [InlineData(10)]
        public void Constructor_DeberiaLanzar_ArgumentOutOfRangeException_ConPuntuacionInvalida(int invalidScore)
        {
            // Arrange & Act & Assert
            var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                new DestinationRating(
                    id: Guid.NewGuid(),
                    destinationId: Guid.NewGuid(),
                    score: invalidScore,
                    comment: null,
                    userId: Guid.NewGuid()
                );
            });

            exception.ParamName.ShouldBe("score");
            exception.Message.ShouldContain("debe estar entre 1 y 5");
        }

        [Fact]
        public void SetScore_DeberiaActualizar_PuntuacionValida()
        {
            // Arrange
            var rating = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: Guid.NewGuid(),
                score: 3,
                comment: null,
                userId: Guid.NewGuid()
            );

            // Act
            rating.SetScore(5);

            // Assert
            rating.Score.ShouldBe(5);
        }

        [Fact]
        public void SetScore_DeberiaLanzar_Excepcion_ConPuntuacionInvalida()
        {
            // Arrange
            var rating = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: Guid.NewGuid(),
                score: 3,
                comment: null,
                userId: Guid.NewGuid()
            );

            // Act & Assert
            Should.Throw<ArgumentOutOfRangeException>(() => rating.SetScore(0));
            Should.Throw<ArgumentOutOfRangeException>(() => rating.SetScore(6));
        }

        #endregion

        #region Comentarios Opcionales

        [Fact]
        public void Constructor_DeberiaAceptar_ComentarioNull()
        {
            // Arrange & Act
            var rating = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: Guid.NewGuid(),
                score: 4,
                comment: null,
                userId: Guid.NewGuid()
            );

            // Assert
            rating.Comment.ShouldBeNull();
        }

        [Fact]
        public void Constructor_DeberiaAceptar_ComentarioVacio()
        {
            // Arrange & Act
            var rating = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: Guid.NewGuid(),
                score: 4,
                comment: "",
                userId: Guid.NewGuid()
            );

            // Assert
            rating.Comment.ShouldBeNull("Comentarios vacíos deben convertirse en null");
        }

        [Fact]
        public void Constructor_DeberiaTrimear_ComentarioConEspacios()
        {
            // Arrange & Act
            var rating = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: Guid.NewGuid(),
                score: 4,
                comment: "   Excelente lugar   ",
                userId: Guid.NewGuid()
            );

            // Assert
            rating.Comment.ShouldBe("Excelente lugar");
        }

        [Fact]
        public void Update_DeberiaPermitir_CambiarComentario()
        {
            // Arrange
            var rating = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: Guid.NewGuid(),
                score: 4,
                comment: "Comentario inicial",
                userId: Guid.NewGuid()
            );

            // Act
            rating.Update(comment: "Comentario actualizado");

            // Assert
            rating.Comment.ShouldBe("Comentario actualizado");
        }

        [Fact]
        public void Update_DeberiaPermitir_EliminarComentario()
        {
            // Arrange
            var rating = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: Guid.NewGuid(),
                score: 4,
                comment: "Comentario inicial",
                userId: Guid.NewGuid()
            );

            // Act
            rating.Update(comment: null);

            // Assert
            rating.Comment.ShouldBeNull();
        }

        [Fact]
        public void Update_DeberiaPermitir_ActualizarPuntuacionYComentario()
        {
            // Arrange
            var rating = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: Guid.NewGuid(),
                score: 3,
                comment: "Regular",
                userId: Guid.NewGuid()
            );

            // Act
            rating.Update(comment: "Mejoró mucho", score: 5);

            // Assert
            rating.Score.ShouldBe(5);
            rating.Comment.ShouldBe("Mejoró mucho");
        }

        #endregion

        #region Validación de Relaciones

        [Fact]
        public void Constructor_DeberiaAsignar_DestinationIdCorrectamente()
        {
            // Arrange
            var destinationId = Guid.NewGuid();

            // Act
            var rating = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: destinationId,
                score: 4,
                comment: null,
                userId: Guid.NewGuid()
            );

            // Assert
            rating.DestinationId.ShouldBe(destinationId);
        }

        [Fact]
        public void Constructor_DeberiaAsignar_UserIdCorrectamente()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var rating = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: Guid.NewGuid(),
                score: 4,
                comment: null,
                userId: userId
            );

            // Assert
            rating.UserId.ShouldBe(userId);
        }

        #endregion

        #region Validación de Duplicados (Lógica Conceptual)

        [Fact]
        public void DosCalificaciones_DelMismoUsuario_DeberianTener_DiferentesIds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var destinationId = Guid.NewGuid();

            // Act
            var rating1 = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: destinationId,
                score: 4,
                comment: "Primera calificación",
                userId: userId
            );

            var rating2 = new DestinationRating(
                id: Guid.NewGuid(),
                destinationId: destinationId,
                score: 5,
                comment: "Segunda calificación",
                userId: userId
            );

            // Assert
            rating1.Id.ShouldNotBe(rating2.Id, "Cada calificación debe tener un ID único");
            rating1.UserId.ShouldBe(rating2.UserId, "Ambas pertenecen al mismo usuario");
            rating1.DestinationId.ShouldBe(rating2.DestinationId, "Ambas son del mismo destino");
        }

        #endregion
    }
}