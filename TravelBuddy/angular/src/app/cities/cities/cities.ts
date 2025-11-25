import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CityDto {
  id: string;
  nombre: string;
  pais?: string; // opcional, por si tu API lo devuelve
}

@Injectable({ providedIn: 'root' })
export class CitiesService {
  constructor(private http: HttpClient) {}

  // Llama a POST /api/app/destino/buscar-por-nombre-externamente
  searchByName(name: string): Observable<CityDto[]> {
    // el body es un simple string o un objeto, según tu API
    return this.http.post<CityDto[]>('/api/app/destino/buscar-por-nombre-externamente', { nombre: name });
  }
}
