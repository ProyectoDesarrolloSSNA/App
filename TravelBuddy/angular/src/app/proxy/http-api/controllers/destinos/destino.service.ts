import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { CityDto, DestinoDto } from '../../../destinos/models';

@Injectable({
  providedIn: 'root',
})
export class DestinoService {
  apiName = 'Default';
  

  buscarPorNombre = (nombre: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoDto[]>({
      method: 'GET',
      url: '/api/app/destinos/buscar-local',
      params: { nombre },
    },
    { apiName: this.apiName,...config });
  

  buscarPorNombreExternamente = (nombre: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CityDto[]>({
      method: 'GET',
      url: '/api/app/destinos/buscar-api',
      params: { nombre },
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
