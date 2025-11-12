using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TravelBuddy.Destinos
{
    public interface IDestinoAppService :
        ICrudAppService<
            DestinoDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateDestinoDto>
    {
        // 🔹 Nuevo método para buscar ciudades en la API externa GeoDB Cities
        Task<List<CityDto>> BuscarPorNombreExternamenteAsync(string nombre);
    }
}
