import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { CreateDestinationRatingDto, DestinationRatingDto } from '../../ratings/dtos/models';

@Injectable({
  providedIn: 'root',
})
export class DestinationRatingService {
  apiName = 'Default';
  

  create = (input: CreateDestinationRatingDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinationRatingDto>({
      method: 'POST',
      url: '/api/app/destination-rating',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getMyRatings = (destinationId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinationRatingDto[]>({
      method: 'GET',
      url: `/api/app/destination-rating/my-ratings/${destinationId}`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
