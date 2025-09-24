using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelBuddy.Destinos
{
    public class DestinoDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Pais { get; set; }
        public string Descripcion { get; set; }
    }

    public class CreateDestinoDto
    {
        public string Nombre { get; set; }
        public string Pais { get; set; }
        public string Descripcion { get; set; }
    }
}
