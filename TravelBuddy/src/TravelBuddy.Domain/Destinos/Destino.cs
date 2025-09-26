using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace TravelBuddy.Destinos
{
    public class Destino : AggregateRoot<Guid>
    {
        public string Nombre { get; private set; } = default!;
        public string Pais { get; private set; } = default!;
        public string Descripcion { get; private set; } = default!;

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
