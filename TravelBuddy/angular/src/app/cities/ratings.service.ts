import { Injectable } from '@angular/core';

export interface Rating {
  cityId: string;
  userName: string;
  stars: number;
  comment: string;
  date: Date;
}

@Injectable({
  providedIn: 'root',
})
export class RatingsService {
  private readonly STORAGE_KEY = 'cityRatings';

  getRatings(cityId: string): Rating[] {
    const allRatings = this.getAllRatings();
    return allRatings.filter(r => r.cityId === cityId);
  }

  getUserRating(cityId: string, userName: string): Rating | undefined {
    const ratings = this.getRatings(cityId);
    return ratings.find(r => r.userName === userName);
  }

  addOrUpdateRating(rating: Rating): void {
    const allRatings = this.getAllRatings();
    const existingIndex = allRatings.findIndex(
      r => r.cityId === rating.cityId && r.userName === rating.userName
    );

    if (existingIndex >= 0) {
      allRatings[existingIndex] = rating;
    } else {
      allRatings.push(rating);
    }

    this.saveRatings(allRatings);
  }

  deleteRating(cityId: string, userName: string): void {
    const allRatings = this.getAllRatings();
    const filtered = allRatings.filter(
      r => !(r.cityId === cityId && r.userName === userName)
    );
    this.saveRatings(filtered);
  }

  getAverageRating(cityId: string): { average: number; count: number } {
    const ratings = this.getRatings(cityId);
    if (ratings.length === 0) {
      return { average: 0, count: 0 };
    }

    const sum = ratings.reduce((acc, r) => acc + r.stars, 0);
    return {
      average: sum / ratings.length,
      count: ratings.length,
    };
  }

  private getAllRatings(): Rating[] {
    const data = localStorage.getItem(this.STORAGE_KEY);
    if (!data) return [];

    try {
      const ratings = JSON.parse(data) as Rating[];
      // Convertir strings de fecha a objetos Date
      return ratings.map(r => ({
        ...r,
        date: new Date(r.date),
      }));
    } catch {
      return [];
    }
  }

  private saveRatings(ratings: Rating[]): void {
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(ratings));
  }
}
