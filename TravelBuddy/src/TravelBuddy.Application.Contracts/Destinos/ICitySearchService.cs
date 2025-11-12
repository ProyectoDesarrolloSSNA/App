using System.Collections.Generic;
using System.Threading.Tasks;

namespace TravelBuddy.Destinos
{
    public interface ICitySearchService
    {
        Task<List<CityDto>> BuscarCiudadesPorNombreAsync(string nombre);
    }
}