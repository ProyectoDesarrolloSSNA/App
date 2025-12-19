using System;
using System.ComponentModel.DataAnnotations;

namespace TravelBuddy.ExperienciasViaje.Dtos
{
    public class CrearActualizarExperienciaViajeDto
    {
        [Required]
        public Guid DestinoId { get; set; }

        [Required]
        [StringLength(128)]
        public string Titulo { get; set; }

        [StringLength(2000)]
        public string Descripcion { get; set; }

        [Range(1, 5)]
        public int Calificacion { get; set; }
    }
}