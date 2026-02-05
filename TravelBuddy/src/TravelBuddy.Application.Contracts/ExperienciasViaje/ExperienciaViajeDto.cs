using System;
using Volo.Abp.Application.Dtos;

namespace TravelBuddy.ExperienciasViaje.Dtos
{
    public class ExperienciaViajeDto : AuditedEntityDto<Guid>
    {
        public Guid DestinoId { get; set; }
        public string DestinoNombre { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Calificacion { get; set; }
    }
}