using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
// usa el namespace que tengas realmente en GeoDbCitySearchService.cs:
using TravelBuddy.Destinos; // o TravelBuddy.Destinos
using Xunit;

namespace TravelBuddy.Application.Tests.External
{
    public class GeoDbCitySearchService_IntegrationTests
    {
        private class FailingHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                throw new HttpRequestException("Simulated network error");
            }
        }

        private GeoDbCitySearchService CreateService()
        {
            var httpClient = new HttpClient();
            return new GeoDbCitySearchService(httpClient);
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task BuscarCiudadesPorNombreAsync_RetornaResultados_ParaNombreValido()
        {
            var service = CreateService();

            var result = await service.BuscarCiudadesPorNombreAsync("Río");

            Assert.NotNull(result);
            Assert.NotEmpty(result);         // result es List<CityDto>
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task BuscarCiudadesPorNombreAsync_Vacio_ParaSinCoincidencia()
        {
            var service = CreateService();

            var result = await service.BuscarCiudadesPorNombreAsync("zzzzzzzzzz");

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task BuscarCiudadesPorNombreAsync_Vacio_ParaInputInvalido()
        {
            var service = CreateService();

            var result = await service.BuscarCiudadesPorNombreAsync("");

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task BuscarCiudadesPorNombreAsync_ManejaErrorDeRed()
        {
            var httpClient = new HttpClient(new FailingHandler());
            var service = new GeoDbCitySearchService(httpClient);

            var result = await service.BuscarCiudadesPorNombreAsync("Rio");

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}