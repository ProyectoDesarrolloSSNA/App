// src/app/cities/cities.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CityDto {
  name: string;
  country: string;
  latitude?: number;
  longitude?: number;
}

@Injectable({ providedIn: 'root' })
export class CitiesService {
  constructor(private http: HttpClient) {}

  // Usa POST con parámetro en query: ?nombre=Paris
  searchByName(name: string): Observable<CityDto[]> {
    const url =
      '/api/app/destino/buscar-por-nombre-externamente?nombre=' +
      encodeURIComponent(name);

    // body = null porque el endpoint no espera body
    return this.http.post<CityDto[]>(url, null);
  }
}
