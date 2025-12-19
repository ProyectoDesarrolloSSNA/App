using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using TravelBuddy.Destinos;

namespace TravelBuddy.ExperienciasViaje
{
    public class ExperienciaViaje : AuditedAggregateRoot<Guid>
    {
        public Guid DestinoId { get; private set; }
        public virtual Destino Destino { get; set; } = default!;
        public string Titulo { get; private set; } = default!;
        public string? Descripcion { get; private set; }
        public int Calificacion { get; private set; }

        protected ExperienciaViaje() { }

        public ExperienciaViaje(Guid id, Guid destinoId, string titulo, string? descripcion, int calificacion)
            : base(id)
        {
            DestinoId = destinoId;
            SetTitulo(titulo);       
            SetDescripcion(descripcion);
            SetCalificacion(calificacion);
        }

        //Métodos de comportamiento
        public void SetTitulo(string titulo)
        {
            // Valida que no sea nulo ni espacios en blanco
            Check.NotNullOrWhiteSpace(titulo, nameof(titulo));
            Titulo = titulo;
        }

        public void SetDescripcion(string? descripcion)
        {
            Descripcion = descripcion;
        }

        public void SetCalificacion(int calificacion)
        {
            if (calificacion < 1 || calificacion > 5)
            {
                throw new UserFriendlyException("La calificación debe estar entre 1 y 5 estrellas.");
            }
            Calificacion = calificacion;
        }
    }
}