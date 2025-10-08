using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelBuddy.Destinos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace TravelBuddy.HttpApi.Controllers.Destinos
{
    [Route("api/app/destinos")]
    public class DestinoController : AbpController
    {
        private readonly IDestinoAppService _appService;

        public DestinoController(IDestinoAppService appService)
        {
            _appService = appService;
        }

        // ✅ Endpoint para búsqueda interna (en tu base local)
        [HttpGet("buscar-local")]
        public async Task<IReadOnlyList<DestinoDto>> BuscarPorNombreAsync([FromQuery] string nombre)
        {
            // GetListAsync devuelve un PagedResultDto, de ahí tomamos sólo los Items
            var result = await _appService.GetListAsync(new PagedAndSortedResultRequestDto());
            return result.Items; // IReadOnlyList<DestinoDto>
        }

        // ✅ Endpoint para búsqueda externa (GeoDB Cities)
        [HttpGet("buscar-api")]
        public async Task<List<CityDto>> BuscarPorNombreExternamenteAsync([FromQuery] string nombre)
        {
            return await _appService.BuscarPorNombreExternamenteAsync(nombre);
        }
    }
}
