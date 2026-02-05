using System;
using Volo.Abp.Application.Dtos;

namespace TravelBuddy.ExperienciasViaje.Dtos
{
    public class GetExperienciasViajeInput : PagedAndSortedResultRequestDto
    {
        public string? TextoFiltro { get; set; }
        public Guid? DestinoId { get; set; }
        public string? Sentimiento { get; set; }
    }
}