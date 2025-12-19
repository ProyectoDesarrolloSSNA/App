using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Local;
using TravelBuddy.Destinos.Events;

namespace TravelBuddy.Destinos
{
    public class DestinoAppService
        : CrudAppService<Destino, DestinoDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateDestinoDto>,
          IDestinoAppService
    {
        private readonly ICitySearchService _citySearchService;

        private readonly ILocalEventBus _localEventBus;
        public DestinoAppService(
            IRepository<Destino, Guid> repo,
            ICitySearchService citySearchService,
            ILocalEventBus localEventBus
        ) : base(repo)
        {
            _citySearchService = citySearchService;
            _localEventBus = localEventBus;
        }

        public override async Task<DestinoDto> UpdateAsync(Guid id, CreateUpdateDestinoDto input)
        {
            var destinoDto = await base.UpdateAsync(id, input);

            // Publicamos el evento de cambio relevante
            await _localEventBus.PublishAsync(new DestinoChangedEto
            {
                DestinoId = destinoDto.Id,
                DestinoNombre = destinoDto.Nombre,
                TipoCambio = "Información Actualizada"
            });

            return destinoDto;
        }

        public async Task<List<CityDto>> BuscarPorNombreExternamenteAsync(string nombre)
        {
            return await _citySearchService.BuscarCiudadesPorNombreAsync(nombre);
        }
    }
}