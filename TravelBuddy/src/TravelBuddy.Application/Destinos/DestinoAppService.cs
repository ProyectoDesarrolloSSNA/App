using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TravelBuddy.Destinos
{
    public class DestinoAppService
        : CrudAppService<Destino, DestinoDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateDestinoDto>,
          IDestinoAppService
    {
        private readonly ICitySearchService _citySearchService;

        public DestinoAppService(
            IRepository<Destino, Guid> repo,
            ICitySearchService citySearchService
        ) : base(repo)
        {
            _citySearchService = citySearchService;
        }

        public async Task<List<CityDto>> BuscarPorNombreExternamenteAsync(string nombre)
        {
            return await _citySearchService.BuscarCiudadesPorNombreAsync(nombre);
        }
    }
}