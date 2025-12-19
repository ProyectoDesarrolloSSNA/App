import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// 👉 Exportamos el tipo que usará el componente
export interface CityDto {
  id: string;
  nombre: string;   // usa el nombre real que devuelve tu API
  pais?: string;
}

// 👉 Exportamos una clase inyectable (el servicio)
@Injectable({ providedIn: 'root' })
export class CitiesService {
  constructor(private http: HttpClient) {}

  // Tu endpoint real (POST) según Swagger:
  // POST /api/app/destino/buscar-por-nombre-externamente
  searchByName(name: string): Observable<CityDto[]> {
    // Si tu API espera { nombre: '...' }, lo mandamos así:
    return this.http.post<CityDto[]>(
      '/api/app/destino/buscar-por-nombre-externamente',
      { nombre: name }
    );
  }
}
