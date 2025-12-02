using System;
using Volo.Abp.Application.Dtos;

namespace TravelBuddy.Administration
{
    public class ApiUsageLogDto : EntityDto<Guid>
    {
        public string ServiceName { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public long ExecutionDurationMs { get; set; }
        public DateTime CreationTime { get; set; }
    }
}