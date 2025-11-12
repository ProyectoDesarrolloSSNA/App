using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace TravelBuddy.Destinos
{
    public class GeoDbCitySearchService : ICitySearchService
    {
        private readonly HttpClient _httpClient;

        public GeoDbCitySearchService(HttpClient httpClient /*, IConfiguration config si luego lo volvés a usar*/)
        {
            _httpClient = httpClient;

            
            const string rapidApiKey = "ba5c2b9a91msh66f2721d0b0cfa8p17b012jsn278568ca92a2";
            const string rapidApiHost = "wft-geo-db.p.rapidapi.com";

            // Seteo de headers (limpio antes por si el HttpClient se reutiliza)
            _httpClient.DefaultRequestHeaders.Remove("X-RapidAPI-Key");
            _httpClient.DefaultRequestHeaders.Remove("X-RapidAPI-Host");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-RapidAPI-Key", rapidApiKey);
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-RapidAPI-Host", rapidApiHost);
        }

        public async Task<List<CityDto>> BuscarCiudadesPorNombreAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return new List<CityDto>();
            }
            // Endpoint recomendado
            var url = $"https://wft-geo-db.p.rapidapi.com/v1/geo/cities?namePrefix={Uri.EscapeDataString(nombre)}&limit=5";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error consultando GeoDB Cities: {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
            }

            var json = await response.Content.ReadFromJsonAsync<GeoDbResponse>();
            if (json?.Data == null) return new List<CityDto>();

            return json.Data.Select(c => new CityDto
            {
                Name = c.City ?? string.Empty,
                Country = c.Country ?? string.Empty,
                Latitude = c.Latitude,
                Longitude = c.Longitude
            }).ToList();
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