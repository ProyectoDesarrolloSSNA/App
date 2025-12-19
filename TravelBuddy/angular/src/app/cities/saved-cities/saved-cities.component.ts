import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CitiesService, CityDto } from '../cities.service';
import { LocalAuthService } from '../../auth/auth.service';
import { RatingsService } from '../ratings.service';

@Component({
  selector: 'app-saved-cities',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './saved-cities.component.html',
  styleUrls: ['./saved-cities.component.scss'],
})
export class SavedCitiesComponent implements OnInit {
  private readonly citiesService = inject(CitiesService);
  private readonly authService = inject(LocalAuthService);
  private readonly ratingsService = inject(RatingsService);
  private readonly router = inject(Router);

  currentUser$ = this.authService.currentUser$;
  savedCityNames: string[] = [];
  loading = false;

  ngOnInit(): void {
    this.loadSavedCities();
  }

  loadSavedCities(): void {
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) {
      this.router.navigate(['/auth/login']);
      return;
    }

    const savedKey = `savedCities_${currentUser.userName}`;
    const savedData = localStorage.getItem(savedKey);

    if (savedData) {
      try {
        this.savedCityNames = JSON.parse(savedData) as string[];
      } catch (e) {
        console.error('Error loading saved cities:', e);
        this.savedCityNames = [];
      }
    }
  }

  removeSavedCity(cityId: string): void {
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) return;

    this.savedCityNames = this.savedCityNames.filter(id => id !== cityId);

    // Guardar en localStorage
    const savedKey = `savedCities_${currentUser.userName}`;
    localStorage.setItem(savedKey, JSON.stringify(this.savedCityNames));
  }

  getCityName(cityId: string): string {
    return cityId.split('_')[0];
  }

  getCityCountry(cityId: string): string {
    return cityId.split('_').slice(1).join('_');
  }

  getCityRating(cityId: string): { average: number; count: number } {
    return this.ratingsService.getAverageRating(cityId);
  }

  getStarsArray(rating: number): number[] {
    return Array(5).fill(0).map((_, i) => i + 1);
  }

  goBack(): void {
    this.router.navigate(['/cities']);
  }
}
