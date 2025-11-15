import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, of } from 'rxjs';
import {
  debounceTime,
  distinctUntilChanged,
  switchMap,
  tap,
  finalize,
} from 'rxjs/operators';
import { CitiesService, CityDto } from '../cities.service';

@Component({
  selector: 'app-search-city',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search-city.html',
  styleUrls: ['./search-city.scss'],
})
export class SearchCityComponent implements OnInit {
  private readonly citiesService = inject(CitiesService);

  term$ = new Subject<string>();
  cities: CityDto[] = [];
  allCities: CityDto[] = [];
  loading = false;
  errorMsg = '';
  
  // Campos de búsqueda
  searchText = '';
  searchCountry = '';
  
  // Paginación
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;

  ngOnInit(): void {
    this.term$
      .pipe(
        debounceTime(400),
        distinctUntilChanged(),
        tap(() => {
          this.loading = true;
          this.errorMsg = '';
        }),
        switchMap(term => {
          const text = term?.trim();
          if (!text) {
            return of([] as CityDto[]).pipe(
              tap(() => (this.loading = false)),
            );
          }
          return this.citiesService
            .buscarPorNombre(text)
            .pipe(finalize(() => (this.loading = false)));
        }),
      )
      .subscribe({
        next: res => {
          this.allCities = res;
          this.applyFiltersAndPagination();
        },
        error: _ =>
          (this.errorMsg =
            'No se pudo buscar. ¿Backend levantado y proxy correcto?'),
      });
  }

  onInput(value: string) {
    this.term$.next(value);
  }

  buscar(): void {
    if (this.searchText.trim()) {
      this.currentPage = 1;
      this.term$.next(this.searchText);
    }
  }

  limpiar(): void {
    this.searchText = '';
    this.searchCountry = '';
    this.currentPage = 1;
    this.allCities = [];
    this.cities = [];
  }

  applyFiltersAndPagination(): void {
    let filtered = [...this.allCities];
    
    // Filtrar por país si se especificó
    if (this.searchCountry.trim()) {
      const countryLower = this.searchCountry.toLowerCase();
      filtered = filtered.filter(city => 
        city.country?.toLowerCase().includes(countryLower)
      );
    }
    
    // Calcular paginación
    this.totalPages = Math.ceil(filtered.length / this.pageSize);
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.cities = filtered.slice(startIndex, endIndex);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.applyFiltersAndPagination();
    }
  }

  goToFirstPage(): void {
    this.goToPage(1);
  }

  goToLastPage(): void {
    this.goToPage(this.totalPages);
  }

  getPaginationPages(): number[] {
    const pages: number[] = [];
    const maxPages = 5;
    let startPage = Math.max(1, this.currentPage - Math.floor(maxPages / 2));
    let endPage = Math.min(this.totalPages, startPage + maxPages - 1);

    if (endPage - startPage < maxPages - 1) {
      startPage = Math.max(1, endPage - maxPages + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    return pages;
  }

  verEnMapa(city: CityDto): void {
    if (city.latitude != null && city.longitude != null) {
      const url = `https://www.google.com/maps/search/?api=1&query=${city.latitude},${city.longitude}`;
      window.open(url, '_blank');
    }
  }

  getCityImage(city: CityDto): string {
    // Aquí puedes implementar la lógica para obtener imágenes desde un servicio
    // Por ahora, usamos Unsplash con el nombre de la ciudad
    const cityName = encodeURIComponent(city.name || 'city');
    return `https://source.unsplash.com/400x250/?${cityName},city`;
  }
}
