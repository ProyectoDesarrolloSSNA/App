using System;
using System.Collections.Generic;
using System.Diagnostics; // Necesario para el cronómetro (Stopwatch)
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TravelBuddy.Administration; // Tu nueva entidad
using Volo.Abp.Domain.Repositories; // Para IRepository

namespace TravelBuddy.Destinos
{
    public class GeoDbCitySearchService : ICitySearchService
    {
        private readonly HttpClient _httpClient;
        // 1. Inyectamos el repositorio de logs
        private readonly IRepository<ApiUsageLog, Guid> _apiLogRepository;

        public GeoDbCitySearchService(
            HttpClient httpClient,
            IRepository<ApiUsageLog, Guid> apiLogRepository)
        {
            _httpClient = httpClient;
            _apiLogRepository = apiLogRepository;

            const string rapidApiKey = "ba5c2b9a91msh66f2721d0b0cfa8p17b012jsn278568ca92a2";
            const string rapidApiHost = "wft-geo-db.p.rapidapi.com";

            // Limpieza y configuración de headers
            _httpClient.DefaultRequestHeaders.Remove("X-RapidAPI-Key");
            _httpClient.DefaultRequestHeaders.Remove("X-RapidAPI-Host");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-RapidAPI-Key", rapidApiKey);
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-RapidAPI-Host", rapidApiHost);
        }

        public async Task<List<CityDto>> BuscarCiudadesPorNombreAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return new List<CityDto>();

            var url = $"https://wft-geo-db.p.rapidapi.com/v1/geo/cities?namePrefix={Uri.EscapeDataString(nombre)}&limit=5";

            // 2. Iniciamos cronómetro para medir rendimiento
            var stopwatch = Stopwatch.StartNew();
            HttpResponseMessage? response = null;

            try
            {
                // Hacemos la llamada real
                response = await _httpClient.GetAsync(url);

                stopwatch.Stop(); // Detenemos cronómetro

                // Verificamos éxito
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    // Logueamos el error antes de lanzar la excepción
                    await LogApiCallAsync(nombre, (int)response.StatusCode, stopwatch.ElapsedMilliseconds);
                    throw new Exception($"Error consultando GeoDB Cities: {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
                }

                var json = await response.Content.ReadFromJsonAsync<GeoDbResponse>();

                // 3. ¡ÉXITO! Guardamos el registro en la base de datos
                await LogApiCallAsync(nombre, (int)response.StatusCode, stopwatch.ElapsedMilliseconds);

                if (json?.Data == null) return new List<CityDto>();

                return json.Data.Select(c => new CityDto
                {
                    Name = c.City ?? string.Empty,
                    Country = c.Country ?? string.Empty,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude
                }).ToList();
            }
            catch (Exception)
            {
                // Si ocurre una excepción de red (ej. sin internet), también queremos saberlo
                if (stopwatch.IsRunning) stopwatch.Stop();

                var status = response != null ? (int)response.StatusCode : 500;
                await LogApiCallAsync(nombre, status, stopwatch.ElapsedMilliseconds);

                throw; // Re-lanzamos la excepción para que la UI la maneje
            }
        }

        // Método auxiliar privado para guardar en la DB
        private async Task LogApiCallAsync(string nombreBuscado, int statusCode, long duration)
        {
            var log = new ApiUsageLog(
                Guid.NewGuid(),
                serviceName: "GeoDB RapidAPI",
                endpoint: "v1/geo/cities",
                parameters: $"query={nombreBuscado}",
                statusCode: statusCode,
                duration: duration
            );

            await _apiLogRepository.InsertAsync(log);
        }

        // DTOs internos para mapear la respuesta de GeoDB
        private class GeoDbResponse
        {
            public List<GeoDbCity> Data { get; set; } = new();
        }

        private class GeoDbCity
        {
            public string? City { get; set; }
            public string? Country { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }
}