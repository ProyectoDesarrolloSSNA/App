using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelBuddy.Destinos
{
    public class DestinoDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = default!;
        public string Pais { get; set; } = default!;
        public string Descripcion { get; set; } = default!;
    }
}
