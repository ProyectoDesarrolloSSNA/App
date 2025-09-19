using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace TravelBuddy.Destinos
{
    public class Destino : Entity<Guid>
    {
        public string Nombre { get; private set; }
        public string Pais { get; private set; }
        public string Descripcion { get; private set; }

        // Constructor vacío requerido por EF Core  
        protected Destino() { }

        public Destino(Guid id, string nombre, string pais, string descripcion) : base(id)
        {
            Nombre = nombre;
            Pais = pais;
            Descripcion = descripcion;
        }
    }
}
