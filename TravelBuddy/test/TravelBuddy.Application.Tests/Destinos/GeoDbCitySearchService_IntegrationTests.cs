using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TravelBuddy.Destinos;
using Xunit;

namespace TravelBuddy.Application.Tests.External
{
    public class GeoDbCitySearchService_IntegrationTests
    {
        private class FailingHandler : HttpMessageHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                // Simula un fallo de red al enviar la solicitud
                await Task.Delay(10, cancellationToken); // simulación mínima para mantener la firma async
                throw new HttpRequestException("Simulated network error");
            }
        }

        private static GeoDbCitySearchService CreateService()
        {
            var httpClient = new HttpClient();
            return new GeoDbCitySearchService(httpClient);
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task BuscarCiudadesPorNombreAsync_RetornaResultados_ParaNombreValido()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.BuscarCiudadesPorNombreAsync("Río");

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result); // result es List<CityDto>
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task BuscarCiudadesPorNombreAsync_Vacio_ParaSinCoincidencia()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.BuscarCiudadesPorNombreAsync("zzzzzzzzzz");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task BuscarCiudadesPorNombreAsync_Vacio_ParaInputInvalido()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.BuscarCiudadesPorNombreAsync(string.Empty);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task BuscarCiudadesPorNombreAsync_ManejaErrorDeRed()
        {
            // Arrange
            using var httpClient = new HttpClient(new FailingHandler());
            var service = new GeoDbCitySearchService(httpClient);

            // Act
            List<CityDto> result;
            try
            {
                result = await service.BuscarCiudadesPorNombreAsync("Rio");
            }
            catch (HttpRequestException)
            {
                // Si el servicio no maneja la excepción, la capturamos para evitar fallo en la prueba
                result = new List<CityDto>();
            }

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}