import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, tap, finalize } from 'rxjs/operators';
import { CitiesService, CityDto } from '../cities.service';

@Component({
  selector: 'app-search-city',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './search-city.html',
  styleUrls: ['./search-city.scss'],
})
export class SearchCityComponent implements OnInit {
  // 👉 inyectamos el servicio así, sin usar el constructor
  private readonly citiesService = inject(CitiesService);

  term$ = new Subject<string>();
  cities: CityDto[] = [];
  loading = false;
  errorMsg = '';

  ngOnInit(): void {
    this.term$
      .pipe(
        debounceTime(400),
        distinctUntilChanged(),
        tap(() => { this.loading = true; this.errorMsg = ''; }),
        switchMap(term => {
          const text = term?.trim();
          if (!text) {
            return of([] as CityDto[]).pipe(tap(() => (this.loading = false)));
          }
          return this.citiesService
            .searchByName(text)
            .pipe(finalize(() => (this.loading = false)));
        })
      )
      .subscribe({
        next: res => (this.cities = res),
        error: _ => (this.errorMsg = 'No se pudo buscar. ¿Backend levantado y proxy correcto?'),
      });
  }

  onInput(value: string) {
    this.term$.next(value);
  }
}
