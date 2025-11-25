import type { EntityDto } from '@abp/ng.core';

export interface CreateDestinationRatingDto {
  destinationId?: string;
  score: number;
  comment?: string;
}

export interface DestinationRatingDto extends EntityDto<string> {
  destinationId?: string;
  score: number;
  comment?: string;
  creationTime?: string;
}
