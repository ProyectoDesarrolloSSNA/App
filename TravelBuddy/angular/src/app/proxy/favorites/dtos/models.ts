import type { EntityDto } from '@abp/ng.core';

export interface AddDestinationToFavoritesDto {
  destinationId: string;
}

export interface DestinationFavoriteDto extends EntityDto<string> {
  destinationId?: string;
  userId?: string;
  creationTime?: string;
  destinationName?: string;
  destinationCountry?: string;
}
