import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { PublicProfileDto } from '../../../users/models';

@Injectable({
  providedIn: 'root',
})
export class PublicProfileService {
  apiName = 'Default';
  

  getById = (userId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PublicProfileDto>({
      method: 'GET',
      url: `/api/users/public-profile/${userId}`,
    },
    { apiName: this.apiName,...config });
  

  getByUserName = (userName: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PublicProfileDto>({
      method: 'GET',
      url: `/api/users/public-profile/by-username/${userName}`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
