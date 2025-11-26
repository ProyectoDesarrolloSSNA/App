using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TravelBuddy.Administration
{
    [Authorize(Roles = "admin")]
    public class AdminAppService : TravelBuddyAppService, IAdminAppService
    {
        private readonly IRepository<ApiUsageLog, Guid> _logRepository;

        public AdminAppService(IRepository<ApiUsageLog, Guid> logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<List<ApiUsageLogDto>> GetRecentLogsAsync()
        {
            // Obtenemos todos los logs (en un caso real con millones de registros, usarías PagedResultRequest)
            var queryable = await _logRepository.GetQueryableAsync();

            var logs = queryable
                .OrderByDescending(x => x.CreationTime)
                .Take(50) // Limitamos a los últimos 50 para no saturar la UI
                .ToList();

            return ObjectMapper.Map<List<ApiUsageLog>, List<ApiUsageLogDto>>(logs);
        }

        public async Task<ApiMetricsDto> GetDailyMetricsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var logs = await _logRepository.GetListAsync(x => x.CreationTime >= today);

            if (!logs.Any())
            {
                return new ApiMetricsDto();
            }

            return new ApiMetricsDto
            {
                TotalRequestsToday = logs.Count,
                SuccessfulRequests = logs.Count(x => x.StatusCode >= 200 && x.StatusCode < 300),
                FailedRequests = logs.Count(x => x.StatusCode >= 400),
                AverageDurationMs = Math.Round(logs.Average(x => x.ExecutionDurationMs), 2)
            };
        }
    }
}
