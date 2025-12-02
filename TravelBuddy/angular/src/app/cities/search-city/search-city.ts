import { Component, OnInit, OnDestroy, HostListener, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, of } from 'rxjs';
import {
  debounceTime,
  distinctUntilChanged,
  switchMap,
  tap,
  finalize,
} from 'rxjs/operators';
import { CitiesService, CityDto } from '../cities.service';
import { LocalAuthService } from '../../auth/auth.service';
import { RatingsService } from '../ratings.service';

@Component({
  selector: 'app-search-city',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search-city.html',
  styleUrls: ['./search-city.scss'],
})
export class SearchCityComponent implements OnInit, OnDestroy {
  private readonly citiesService = inject(CitiesService);
  private readonly authService = inject(LocalAuthService);
  private readonly ratingsService = inject(RatingsService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly router = inject(Router);

  // Usuario actual
  currentUser$ = this.authService.currentUser$;
  showProfileMenu = false;

  // Modal de calificación
  showRatingModal = false;
  selectedCity: CityDto | null = null;
  ratingStars = 0;
  ratingComment = '';

  // Modal de comentarios
  showCommentsModal = false;
  cityComments: any[] = [];

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

  // Favoritos
  savedCities: Map<string, boolean> = new Map();

  // Cache de ciudades
  private citiesCache: CityDto[] = [];

  ngOnInit(): void {
    this.loadSavedCities();
    this.term$
      .pipe(
        debounceTime(400),
        distinctUntilChanged(),
        tap((term) => {
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
            .buscarPorNombre(text, this.searchCountry)
            .pipe(finalize(() => (this.loading = false)));
        }),
      )
      .subscribe({
        next: res => {
          this.allCities = res;
          this.citiesCache = res;
          this.currentPage = 1;
          this.applyFiltersAndPagination();
          this.cdr.markForCheck();
        },
        error: (err) => {
          this.errorMsg =
            'No se pudo buscar. ¿Backend levantado y proxy correcto?';
        },
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
    const filtered = [...this.allCities];
    
    // Calcular paginación
    this.totalPages = Math.ceil(filtered.length / this.pageSize);
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.cities = filtered.slice(startIndex, endIndex);
    
    this.cdr.markForCheck();
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

  toggleProfileMenu(): void {
    this.showProfileMenu = !this.showProfileMenu;
  }

  goToProfile(): void {
    this.router.navigate(['/users/profile']);
    this.showProfileMenu = false;
  }

  goToEditProfile(): void {
    this.router.navigate(['/users/edit-profile']);
    this.showProfileMenu = false;
  }

  goToFavorites(): void {
    this.router.navigate(['/cities/favorites']);
    this.showProfileMenu = false;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    const profileMenu = document.querySelector('.profile-menu-wrapper');
    
    // Si el clic está fuera del menú de perfil, cerrar el dropdown
    if (profileMenu && !profileMenu.contains(target)) {
      this.showProfileMenu = false;
    }
  }

  ngOnDestroy(): void {
    // Limpiar si es necesario
  }

  getCityImage(city: CityDto): string {
    // Aquí puedes implementar la lógica para obtener imágenes desde un servicio
    // Por ahora, usamos Unsplash con el nombre de la ciudad
    const cityName = encodeURIComponent(city.name || 'city');
    return `https://source.unsplash.com/400x250/?${cityName},city`;
  }

  loadSavedCities(): void {
    const currentUser = this.authService.getCurrentUser();
    if (currentUser) {
      const savedKey = `savedCities_${currentUser.userName}`;
      const savedData = localStorage.getItem(savedKey);
      if (savedData) {
        try {
          const savedIds = JSON.parse(savedData) as string[];
          this.savedCities.clear();
          savedIds.forEach(id => this.savedCities.set(id, true));
        } catch (e) {
          console.error('Error loading saved cities:', e);
        }
      }
    }
  }

  isCitySaved(cityId: string): boolean {
    return this.savedCities.get(cityId) || false;
  }

  toggleSaveCity(city: CityDto, event: Event): void {
    event.stopPropagation();
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) {
      alert('Debes iniciar sesión para guardar destinos');
      return;
    }

    const cityId = `${city.name}_${city.country}`;
    const savedKey = `savedCities_${currentUser.userName}`;
    
    console.log('Before toggle - savedCities:', Array.from(this.savedCities.keys()));
    console.log('CityId:', cityId);
    console.log('Is saved:', this.isCitySaved(cityId));
    
    if (this.isCitySaved(cityId)) {
      this.savedCities.delete(cityId);
    } else {
      this.savedCities.set(cityId, true);
    }

    // Guardar en localStorage
    const savedIds = Array.from(this.savedCities.keys());
    localStorage.setItem(savedKey, JSON.stringify(savedIds));
    
    console.log('After toggle - savedCities:', savedIds);
    
    // Forzar detección de cambios
    this.cdr.markForCheck();
  }

  openRatingModal(city: CityDto, event: Event): void {
    event.stopPropagation();
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) {
      alert('Debes iniciar sesión para calificar destinos');
      return;
    }

    this.selectedCity = city;
    const cityId = `${city.name}_${city.country}`;
    const existingRating = this.ratingsService.getUserRating(cityId, currentUser.userName);

    if (existingRating) {
      this.ratingStars = existingRating.stars;
      this.ratingComment = existingRating.comment;
    } else {
      this.ratingStars = 0;
      this.ratingComment = '';
    }

    this.showRatingModal = true;
  }

  closeRatingModal(): void {
    this.showRatingModal = false;
    this.selectedCity = null;
    this.ratingStars = 0;
    this.ratingComment = '';
  }

  setRating(stars: number): void {
    this.ratingStars = stars;
  }

  submitRating(): void {
    if (!this.selectedCity || this.ratingStars === 0) {
      alert('Por favor selecciona una calificación');
      return;
    }

    if (!this.ratingComment.trim()) {
      alert('Por favor escribe un comentario');
      return;
    }

    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) return;

    const cityId = `${this.selectedCity.name}_${this.selectedCity.country}`;
    
    this.ratingsService.addOrUpdateRating({
      cityId,
      userName: currentUser.userName,
      stars: this.ratingStars,
      comment: this.ratingComment.trim(),
      date: new Date(),
    });

    this.closeRatingModal();
    this.cdr.markForCheck();
  }

  getCityRating(city: CityDto): { average: number; count: number } {
    const cityId = `${city.name}_${city.country}`;
    return this.ratingsService.getAverageRating(cityId);
  }

  getStarsArray(rating: number): number[] {
    return Array(5).fill(0).map((_, i) => i + 1);
  }

  openCommentsModal(city: CityDto, event: Event): void {
    event.stopPropagation();
    this.selectedCity = city;
    const cityId = `${city.name}_${city.country}`;
    this.cityComments = this.ratingsService.getRatings(cityId);
    this.showCommentsModal = true;
  }

  closeCommentsModal(): void {
    this.showCommentsModal = false;
    this.selectedCity = null;
    this.cityComments = [];
  }

  deleteComment(comment: any, event: Event): void {
    event.stopPropagation();
    if (confirm('¿Estás seguro de que quieres eliminar tu calificación y comentario?')) {
      this.ratingsService.deleteRating(comment.cityId, comment.userName);
      
      // Actualizar la lista de comentarios
      if (this.selectedCity) {
        const cityId = `${this.selectedCity.name}_${this.selectedCity.country}`;
        this.cityComments = this.ratingsService.getRatings(cityId);
      }
      
      this.cdr.markForCheck();
    }
  }

  isOwnComment(comment: any): boolean {
    const currentUser = this.authService.getCurrentUser();
    return currentUser ? comment.userName === currentUser.userName : false;
  }
}
