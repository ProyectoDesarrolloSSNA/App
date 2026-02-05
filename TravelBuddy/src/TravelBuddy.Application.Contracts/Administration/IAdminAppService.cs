using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TravelBuddy.Administration
{
    public interface IAdminAppService : IApplicationService
    {
        Task<List<ApiUsageLogDto>> GetRecentLogsAsync();
        Task<ApiMetricsDto> GetDailyMetricsAsync();
    }
}
