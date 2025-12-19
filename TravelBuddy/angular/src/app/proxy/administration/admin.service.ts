import type { ApiMetricsDto, ApiUsageLogDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  apiName = 'Default';
  

  getDailyMetrics = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ApiMetricsDto>({
      method: 'GET',
      url: '/api/app/admin/daily-metrics',
    },
    { apiName: this.apiName,...config });
  

  getRecentLogs = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ApiUsageLogDto[]>({
      method: 'GET',
      url: '/api/app/admin/recent-logs',
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
