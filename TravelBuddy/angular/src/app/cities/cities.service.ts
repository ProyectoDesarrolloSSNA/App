import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CityDto {
  name: string;
  country: string;
  latitude?: number;
  longitude?: number;
}

@Injectable({
  providedIn: 'root',
})
export class CitiesService {
  constructor(private http: HttpClient) {}

  buscarPorNombre(nombre: string, pais?: string): Observable<CityDto[]> {
    const params: any = { nombre };
    if (pais && pais.trim()) {
      params.pais = pais;
    }
    
    return this.http.post<CityDto[]>(
      '/api/app/destino/buscar-por-nombre-externamente',
      {},
      { params }
    );
  }
}
