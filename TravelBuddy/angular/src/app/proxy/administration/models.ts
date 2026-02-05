import type { EntityDto } from '@abp/ng.core';

export interface ApiMetricsDto {
  totalRequestsToday: number;
  successfulRequests: number;
  failedRequests: number;
  averageDurationMs: number;
}

export interface ApiUsageLogDto extends EntityDto<string> {
  serviceName?: string;
  endpoint?: string;
  statusCode: number;
  executionDurationMs: number;
  creationTime?: string;
}
