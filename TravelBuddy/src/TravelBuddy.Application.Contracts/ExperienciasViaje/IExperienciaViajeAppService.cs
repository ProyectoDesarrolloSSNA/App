using System;
using Volo.Abp.Application.Services;
using TravelBuddy.ExperienciasViaje.Dtos;

namespace TravelBuddy.ExperienciasViaje
{
    public interface IExperienciaViajeAppService :
        ICrudAppService<
            ExperienciaViajeDto,
            Guid,
            GetExperienciasViajeInput,
            CrearActualizarExperienciaViajeDto>
    {
        // Como heredamos de ICrudAppService, ya tenemos definidos implícitamente:
        // Task<ExperienciaViajeDto> CreateAsync(...);
        // Task<ExperienciaViajeDto> UpdateAsync(...);
        // Task DeleteAsync(...);
        // Task<PagedResultDto<ExperienciaViajeDto>> GetListAsync(...);
    }
}