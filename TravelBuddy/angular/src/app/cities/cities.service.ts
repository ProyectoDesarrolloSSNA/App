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

  // 👇 ESTE es el método correcto
  buscarPorNombre(nombre: string): Observable<CityDto[]> {
    return this.http.post<CityDto[]>(
      '/api/app/destino/buscar-por-nombre-externamente',
      {}, // body vacío porque es POST
      {
        params: { nombre },
      }
    );
  }
}
