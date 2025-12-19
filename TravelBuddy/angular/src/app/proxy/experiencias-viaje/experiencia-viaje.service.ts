import type { CrearActualizarExperienciaViajeDto, ExperienciaViajeDto, GetExperienciasViajeInput } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ExperienciaViajeService {
  apiName = 'Default';
  

  create = (input: CrearActualizarExperienciaViajeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ExperienciaViajeDto>({
      method: 'POST',
      url: '/api/app/experiencia-viaje',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/experiencia-viaje/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ExperienciaViajeDto>({
      method: 'GET',
      url: `/api/app/experiencia-viaje/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetExperienciasViajeInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ExperienciaViajeDto>>({
      method: 'GET',
      url: '/api/app/experiencia-viaje',
      params: { textoFiltro: input.textoFiltro, destinoId: input.destinoId, sentimiento: input.sentimiento, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CrearActualizarExperienciaViajeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ExperienciaViajeDto>({
      method: 'PUT',
      url: `/api/app/experiencia-viaje/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
