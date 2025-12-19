using System;

namespace TravelBuddy.Destinos.Events
{
    public class DestinoChangedEto
    {
        public Guid DestinoId { get; set; }
        public string DestinoNombre { get; set; } = string.Empty;
        public string TipoCambio { get; set; } = string.Empty;
    }
}
