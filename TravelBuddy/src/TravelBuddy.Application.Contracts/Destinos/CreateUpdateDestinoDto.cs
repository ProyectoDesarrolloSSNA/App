using System;
using System.ComponentModel.DataAnnotations;

namespace TravelBuddy.Destinos
{
    public class CreateUpdateDestinoDto
    {
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = default!;

        [Required]
        [StringLength(100)]
        public string Pais { get; set; } = default!;

        [StringLength(500)]
        public string Descripcion { get; set; } = default!;
    }
}
