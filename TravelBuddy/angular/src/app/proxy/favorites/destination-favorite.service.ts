import type { AddDestinationToFavoritesDto, DestinationFavoriteDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DestinationFavoriteService {
  apiName = 'Default';
  

  addToFavorites = (input: AddDestinationToFavoritesDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinationFavoriteDto>({
      method: 'POST',
      url: '/api/app/destination-favorite/to-favorites',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getMyFavorites = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinationFavoriteDto[]>({
      method: 'GET',
      url: '/api/app/destination-favorite/my-favorites',
    },
    { apiName: this.apiName,...config });
  

  isInFavorites = (destinationId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, boolean>({
      method: 'POST',
      url: `/api/app/destination-favorite/is-in-favorites/${destinationId}`,
    },
    { apiName: this.apiName,...config });
  

  removeFromFavorites = (destinationId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/destination-favorite/from-favorites/${destinationId}`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
