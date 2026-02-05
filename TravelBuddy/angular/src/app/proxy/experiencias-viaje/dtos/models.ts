import type { AuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CrearActualizarExperienciaViajeDto {
  destinoId: string;
  titulo: string;
  descripcion?: string;
  calificacion: number;
}

export interface ExperienciaViajeDto extends AuditedEntityDto<string> {
  destinoId?: string;
  destinoNombre: string;
  titulo: string;
  descripcion?: string;
  calificacion: number;
}

export interface GetExperienciasViajeInput extends PagedAndSortedResultRequestDto {
  textoFiltro?: string;
  destinoId?: string;
  sentimiento?: string;
}
