using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace TravelBuddy.Administration
{
    public class ApiUsageLog : CreationAuditedEntity<Guid>
    {
        public string ServiceName { get; set; } // Ej: "GeoDB"
        public string Endpoint { get; set; }    // Ej: "BuscarCiudades"
        public string Parameters { get; set; }  // Ej: "nombre=Paris"
        public int StatusCode { get; set; }     // Ej: 200, 404, 500
        public long ExecutionDurationMs { get; set; } // Tiempo que tardó

        protected ApiUsageLog() { }

        public ApiUsageLog(Guid id, string serviceName, string endpoint, string parameters, int statusCode, long duration)
            : base(id)
        {
            ServiceName = serviceName;
            Endpoint = endpoint;
            Parameters = parameters;
            StatusCode = statusCode;
            ExecutionDurationMs = duration;
        }
    }
}