import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface UserProfile {
  id: string;
  userName: string;
  email: string;
  name: string;
  surname: string;
  phoneNumber?: string;
  profilePictureUrl?: string;
  bio?: string;
  preferences?: UserPreferences;
  createdAt: Date;
}

export interface UserPreferences {
  theme: 'light' | 'dark';
  notifications: boolean;
  language: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl = `${environment.apis.default.url}/api/app/users`;

  constructor(private http: HttpClient) {}

  // 1.3 Actualizar datos de perfil
  updateProfile(userId: string, profile: Partial<UserProfile>): Observable<UserProfile> {
    return this.http.put<UserProfile>(`${this.apiUrl}/${userId}`, profile);
  }

  // 1.3 Subir foto de perfil
  uploadProfilePicture(userId: string, file: File): Observable<{ url: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ url: string }>(`${this.apiUrl}/${userId}/profile-picture`, formData);
  }

  // 1.3 Actualizar preferencias
  updatePreferences(userId: string, preferences: UserPreferences): Observable<UserProfile> {
    return this.http.put<UserProfile>(`${this.apiUrl}/${userId}/preferences`, preferences);
  }

  // 1.4 Cambiar contraseña
  changePassword(request: ChangePasswordRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/change-password`, request);
  }

  // 1.5 Eliminar cuenta
  deleteAccount(userId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${userId}`);
  }

  // 1.6 Obtener perfil público de otros usuarios
  getPublicProfile(userId: string): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.apiUrl}/${userId}/public`);
  }

  // Obtener perfil actual del usuario autenticado
  getCurrentProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.apiUrl}/me`);
  }
}
