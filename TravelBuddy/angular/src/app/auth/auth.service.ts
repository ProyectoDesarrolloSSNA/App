import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface MockUser {
  id: string;
  userName: string;
  email: string;
  name: string;
  surname: string;
  token: string;
}

@Injectable({
  providedIn: 'root',
})
export class LocalAuthService {
  private currentUserSubject = new BehaviorSubject<MockUser | null>(
    this.getUserFromStorage()
  );
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor() {}

  private getUserFromStorage(): MockUser | null {
    const user = localStorage.getItem('currentUser');
    return user ? JSON.parse(user) : null;
  }

  /**
   * Mock login - simula autenticación local
   */
  login(userName: string, password: string): Observable<MockUser> {
    return new Observable((observer) => {
      setTimeout(() => {
        const mockUser: MockUser = {
          id: '123e4567-e89b-12d3-a456-426614174000',
          userName,
          email: `${userName}@travelbuddy.com`,
          name: 'Usuario',
          surname: 'Demo',
          token: 'mock_token_' + Date.now(),
        };

        localStorage.setItem('currentUser', JSON.stringify(mockUser));
        this.currentUserSubject.next(mockUser);
        observer.next(mockUser);
        observer.complete();
      }, 500);
    });
  }

  /**
   * Mock register - simula registro local
   */
  register(
    userName: string,
    email: string,
    password: string,
    name: string,
    surname: string
  ): Observable<MockUser> {
    return new Observable((observer) => {
      setTimeout(() => {
        const mockUser: MockUser = {
          id: 'user-' + Date.now(),
          userName,
          email,
          name,
          surname,
          token: 'mock_token_' + Date.now(),
        };

        localStorage.setItem('currentUser', JSON.stringify(mockUser));
        this.currentUserSubject.next(mockUser);
        observer.next(mockUser);
        observer.complete();
      }, 500);
    });
  }

  /**
   * Logout
   */
  logout(): void {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }

  /**
   * Obtener usuario actual
   */
  getCurrentUser(): MockUser | null {
    return this.currentUserSubject.value;
  }

  /**
   * Verificar si está autenticado
   */
  isAuthenticated(): boolean {
    return !!this.currentUserSubject.value;
  }
}
