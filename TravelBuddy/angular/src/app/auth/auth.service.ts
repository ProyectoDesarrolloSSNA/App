import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';

export interface MockUser {
  id: string;
  userName: string;
  email: string;
  name: string;
  surname: string;
  password?: string;
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

  // Base de datos local de usuarios registrados
  private registeredUsers: MockUser[] = [];

  constructor() {
    this.initializeDefaultUsers();
  }

  /**
   * Inicializar usuarios de prueba
   */
  private initializeDefaultUsers(): void {
    const stored = localStorage.getItem('registeredUsers');
    if (stored) {
      this.registeredUsers = JSON.parse(stored);
    } else {
      // Usuarios de prueba iniciales
      this.registeredUsers = [
        {
          id: '123e4567-e89b-12d3-a456-426614174000',
          userName: 'juan',
          email: 'juan@travelbuddy.com',
          name: 'Juan',
          surname: 'Pérez',
          password: '123456',
          token: 'mock_token_juan',
        },
        {
          id: '223e4567-e89b-12d3-a456-426614174001',
          userName: 'maria',
          email: 'maria@travelbuddy.com',
          name: 'María',
          surname: 'García',
          password: '123456',
          token: 'mock_token_maria',
        },
        {
          id: '323e4567-e89b-12d3-a456-426614174002',
          userName: 'demo',
          email: 'demo@travelbuddy.com',
          name: 'Demo',
          surname: 'User',
          password: 'demo123',
          token: 'mock_token_demo',
        },
      ];
      localStorage.setItem('registeredUsers', JSON.stringify(this.registeredUsers));
    }
  }

  private getUserFromStorage(): MockUser | null {
    const user = localStorage.getItem('currentUser');
    return user ? JSON.parse(user) : null;
  }

  /**
   * Mock login - valida credenciales contra usuarios registrados
   */
  login(userName: string, password: string): Observable<MockUser> {
    return new Observable((observer) => {
      setTimeout(() => {
        // Buscar usuario registrado
        const user = this.registeredUsers.find(
          (u) => u.userName.toLowerCase() === userName.toLowerCase()
        );

        if (!user) {
          // Usuario no existe
          observer.error({
            message: 'Usuario no encontrado',
            type: 'USER_NOT_FOUND',
          });
          return;
        }

        if (user.password !== password) {
          // Contraseña incorrecta
          observer.error({
            message: 'Contraseña incorrecta',
            type: 'INVALID_PASSWORD',
          });
          return;
        }

        // Login exitoso
        const loginUser: MockUser = {
          id: user.id,
          userName: user.userName,
          email: user.email,
          name: user.name,
          surname: user.surname,
          token: 'mock_token_' + Date.now(),
        };

        localStorage.setItem('currentUser', JSON.stringify(loginUser));
        this.currentUserSubject.next(loginUser);
        observer.next(loginUser);
        observer.complete();
      }, 500);
    });
  }

  /**
   * Mock register - registra nuevo usuario
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
        // Validar que el usuario no exista
        const existingUser = this.registeredUsers.find(
          (u) => u.userName.toLowerCase() === userName.toLowerCase()
        );

        if (existingUser) {
          observer.error({
            message: 'El usuario ya existe',
            type: 'USER_ALREADY_EXISTS',
          });
          return;
        }

        // Validar email único
        const existingEmail = this.registeredUsers.find(
          (u) => u.email.toLowerCase() === email.toLowerCase()
        );

        if (existingEmail) {
          observer.error({
            message: 'El email ya está registrado',
            type: 'EMAIL_ALREADY_EXISTS',
          });
          return;
        }

        // Crear nuevo usuario
        const newUser: MockUser = {
          id: 'user-' + Date.now(),
          userName,
          email,
          name,
          surname,
          password,
          token: 'mock_token_' + Date.now(),
        };

        // Guardar usuario
        this.registeredUsers.push(newUser);
        localStorage.setItem('registeredUsers', JSON.stringify(this.registeredUsers));

        // Auto-login
        const loginUser: MockUser = {
          ...newUser,
          password: undefined,
          token: 'mock_token_' + Date.now(),
        };

        localStorage.setItem('currentUser', JSON.stringify(loginUser));
        
        // IMPORTANTE: Guardar también el perfil completo
        const userProfile = {
          id: newUser.id,
          userName: newUser.userName,
          email: newUser.email,
          name: newUser.name,
          surname: newUser.surname,
          phoneNumber: '',
          profilePictureUrl: null,
          bio: '',
          createdAt: new Date(),
        };
        localStorage.setItem('userProfile', JSON.stringify(userProfile));

        this.currentUserSubject.next(loginUser);
        observer.next(loginUser);
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
   * Eliminar cuenta
   */
  deleteAccount(userId: string, userName: string): Observable<void> {
    return new Observable((observer) => {
      setTimeout(() => {
        // Buscar y eliminar el usuario de la lista registrada
        const index = this.registeredUsers.findIndex((u) => u.id === userId || u.userName === userName);
        
        if (index > -1) {
          this.registeredUsers.splice(index, 1);
          localStorage.setItem('registeredUsers', JSON.stringify(this.registeredUsers));
        }

        // Limpiar datos del usuario actual
        localStorage.removeItem('currentUser');
        
        // Eliminar el perfil específico de este usuario
        const userProfileKey = `userProfile_${userName}`;
        localStorage.removeItem(userProfileKey);
        
        this.currentUserSubject.next(null);

        observer.next();
        observer.complete();
      }, 500);
    });
  }

  /**
   * Verificar si está autenticado
   */
  isAuthenticated(): boolean {
    return !!this.currentUserSubject.value;
  }
}