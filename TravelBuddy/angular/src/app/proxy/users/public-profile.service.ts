import type { PublicProfileDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PublicProfileService {
  apiName = 'Default';
  

  getPublicProfile = (userId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PublicProfileDto>({
      method: 'GET',
      url: `/api/app/public-profile/public-profile/${userId}`,
    },
    { apiName: this.apiName,...config });
  

  getPublicProfileByUserName = (userName: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PublicProfileDto>({
      method: 'GET',
      url: '/api/app/public-profile/public-profile-by-user-name',
      params: { userName },
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
