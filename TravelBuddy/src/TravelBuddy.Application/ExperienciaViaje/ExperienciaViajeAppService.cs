using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using TravelBuddy.ExperienciasViaje.Dtos;

namespace TravelBuddy.ExperienciasViaje
{
   [Authorize] //Solo usuarios logueados entran
    public class ExperienciaViajeAppService : ApplicationService, IExperienciaViajeAppService
    {
        private readonly IRepository<ExperienciaViaje, Guid> _repositorio;

        public ExperienciaViajeAppService(IRepository<ExperienciaViaje, Guid> repositorio)
        {
            _repositorio = repositorio;
        }

        // REQ 4.1: Crear
        public async Task<ExperienciaViajeDto> CreateAsync(CrearActualizarExperienciaViajeDto input)
        {
            var experiencia = new ExperienciaViaje(
                GuidGenerator.Create(),
                input.DestinoId,
                input.Titulo,
                input.Descripcion,
                input.Calificacion
            );

            await _repositorio.InsertAsync(experiencia);

            return ObjectMapper.Map<ExperienciaViaje, ExperienciaViajeDto>(experiencia);
        }

        // REQ 4.2: Editar (Validando propiedad)
        public async Task<ExperienciaViajeDto> UpdateAsync(Guid id, CrearActualizarExperienciaViajeDto input)
        {
            var experiencia = await _repositorio.GetAsync(id);

            // Es mía la experiencia?
            if (experiencia.CreatorId != CurrentUser.Id)
            {
                throw new Volo.Abp.UserFriendlyException("No tienes permiso para editar esta experiencia.");
            }
      
            experiencia.SetTitulo(input.Titulo);
            experiencia.SetDescripcion(input.Descripcion);
            experiencia.SetCalificacion(input.Calificacion);

            await _repositorio.UpdateAsync(experiencia);

            return ObjectMapper.Map<ExperienciaViaje, ExperienciaViajeDto>(experiencia);
        }

        // REQ 4.3: Eliminar (Validando propiedad)
        public async Task DeleteAsync(Guid id)
        {
            var experiencia = await _repositorio.GetAsync(id);

            if (experiencia.CreatorId != CurrentUser.Id)
            {
                throw new Volo.Abp.UserFriendlyException("No tienes permiso para eliminar esta experiencia.");
            }
         
            await _repositorio.DeleteAsync(experiencia);
        }

        public async Task<ExperienciaViajeDto> GetAsync(Guid id)
        {
            var experiencia = await _repositorio.GetAsync(id);
            return ObjectMapper.Map<ExperienciaViaje, ExperienciaViajeDto>(experiencia);
        }

        // REQ 4.4, 4.5, 4.6: Listar con Filtros
        public async Task<PagedResultDto<ExperienciaViajeDto>> GetListAsync(GetExperienciasViajeInput input)
        {
            // Traemos la relación 'Destino' para poder mapear el nombre después
            var query = await _repositorio.WithDetailsAsync(x => x.Destino);

            // Filtros
            if (input.DestinoId.HasValue)
            {
                query = query.Where(x => x.DestinoId == input.DestinoId.Value);
            }

            if (!string.IsNullOrWhiteSpace(input.TextoFiltro))
            {
                query = query.Where(x => x.Titulo.Contains(input.TextoFiltro) ||
                                         x.Descripcion.Contains(input.TextoFiltro));
            }

            if (!string.IsNullOrWhiteSpace(input.Sentimiento))
            {
                var s = input.Sentimiento.ToLower();
                if (s == "positiva") query = query.Where(x => x.Calificacion >= 4);
                else if (s == "negativa") query = query.Where(x => x.Calificacion <= 2);
                else if (s == "neutral") query = query.Where(x => x.Calificacion == 3);
            }

            // Paginación y Orden
            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query.OrderByDescending(x => x.CreationTime)
                         .PageBy(input.SkipCount, input.MaxResultCount);

            var items = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<ExperienciaViajeDto>(
                totalCount,
                ObjectMapper.Map<List<ExperienciaViaje>, List<ExperienciaViajeDto>>(items)
            );
        }
    }
}