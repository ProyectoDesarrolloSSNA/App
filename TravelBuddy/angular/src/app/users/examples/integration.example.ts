import { Component, OnInit, Injectable } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { UserService, UserProfile } from '../services/user.service';

/**
 * Ejemplo de integración del módulo de gestión de usuarios en otros componentes.
 * Este componente demuestra cómo acceder y utilizar las funcionalidades de usuario.
 */
@Component({
  selector: 'app-user-menu-example',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="user-menu">
      <button class="menu-button" (click)="toggleMenu()">
        <img *ngIf="currentUser?.profilePictureUrl" 
             [src]="currentUser.profilePictureUrl" 
             alt="Foto de perfil"
             class="profile-picture">
        <span *ngIf="!currentUser?.profilePictureUrl" class="initial">
          {{ (currentUser?.name?.charAt(0) || '') + (currentUser?.surname?.charAt(0) || '') }}
        </span>
        <span class="user-name">{{ currentUser?.name }}</span>
        <span class="dropdown-icon">▼</span>
      </button>

      <div *ngIf="menuOpen" class="menu-dropdown" @fadeIn>
        <div class="menu-header">
          <p class="user-email">{{ currentUser?.email }}</p>
        </div>

        <nav class="menu-items">
          <a routerLink="/users/profile" class="menu-item" (click)="closeMenu()">
            👤 Mi Perfil
          </a>
          <a routerLink="/users/edit-profile" class="menu-item" (click)="closeMenu()">
            ✏️ Editar Perfil
          </a>
          <a routerLink="/users/change-password" class="menu-item" (click)="closeMenu()">
            🔐 Cambiar Contraseña
          </a>
          <hr>
          <button class="menu-item logout" (click)="logout()">
            🚪 Cerrar Sesión
          </button>
        </nav>
      </div>
    </div>
  `,
  styles: [
    `
      .user-menu {
        position: relative;
      }

      .menu-button {
        display: flex;
        align-items: center;
        gap: 8px;
        padding: 8px 12px;
        background: none;
        border: 1px solid #ddd;
        border-radius: 4px;
        cursor: pointer;
        font-size: 14px;
        transition: all 0.3s ease;
      }

      .menu-button:hover {
        border-color: #007bff;
        background-color: #f8f9fa;
      }

      .profile-picture {
        width: 32px;
        height: 32px;
        border-radius: 50%;
        object-fit: cover;
      }

      .initial {
        width: 32px;
        height: 32px;
        border-radius: 50%;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        display: flex;
        align-items: center;
        justify-content: center;
        font-weight: bold;
        font-size: 12px;
      }

      .user-name {
        font-weight: 500;
        color: #333;
      }

      .dropdown-icon {
        font-size: 10px;
        color: #999;
      }

      .menu-dropdown {
        position: absolute;
        top: 100%;
        right: 0;
        margin-top: 8px;
        background: white;
        border: 1px solid #ddd;
        border-radius: 4px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
        z-index: 1000;
        min-width: 200px;
      }

      .menu-header {
        padding: 12px 16px;
        border-bottom: 1px solid #eee;
      }

      .user-email {
        margin: 0;
        font-size: 12px;
        color: #999;
      }

      .menu-items {
        list-style: none;
        padding: 0;
        margin: 0;
      }

      .menu-item {
        display: block;
        width: 100%;
        padding: 10px 16px;
        border: none;
        background: none;
        text-align: left;
        cursor: pointer;
        font-size: 14px;
        color: #333;
        transition: background-color 0.3s ease;
        text-decoration: none;
      }

      .menu-item:hover {
        background-color: #f8f9fa;
        color: #007bff;
      }

      .menu-item.logout {
        color: #dc3545;
        border-top: 1px solid #eee;
      }

      .menu-item.logout:hover {
        background-color: #fff5f5;
      }

      hr {
        margin: 0;
        border: none;
        border-top: 1px solid #eee;
      }
    `,
  ],
})
export class UserMenuExampleComponent implements OnInit {
  currentUser: UserProfile | null = null;
  menuOpen = false;

  constructor(private userService: UserService, private router: Router) {}

  ngOnInit(): void {
    this.loadCurrentUser();
  }

  loadCurrentUser(): void {
    this.userService.getCurrentProfile().subscribe({
      next: (profile) => {
        this.currentUser = profile;
      },
      error: (error) => {
        console.error('Error loading user profile:', error);
      },
    });
  }

  toggleMenu(): void {
    this.menuOpen = !this.menuOpen;
  }

  closeMenu(): void {
    this.menuOpen = false;
  }

  logout(): void {
    // Implementar lógica de cierre de sesión
    // this.authService.logout().subscribe(() => {
    //   this.router.navigate(['/']);
    // });
    this.closeMenu();
  }
}

/**
 * Ejemplo de uso en un componente que necesita acceder al perfil del usuario actual
 */
@Component({
  selector: 'app-user-profile-consumer',
  standalone: true,
  template: `<div>{{ userBio }}</div>`,
})
export class UserProfileConsumerExample implements OnInit {
  userBio: string | null = null;

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    // Obtener el perfil del usuario actual
    this.userService.getCurrentProfile().subscribe({
      next: (profile) => {
        this.userBio = profile.bio;
      },
      error: (error) => {
        console.error('Error:', error);
      },
    });
  }
}

/**
 * Ejemplo de cómo ver el perfil público de otro usuario
 */
@Injectable({ providedIn: 'root' })
export class ViewPublicProfileExample {
  constructor(private router: Router) {}

  viewUserProfile(userId: string): void {
    // Navegar al perfil público del usuario
    this.router.navigate(['/users/public', userId]);
  }

  // Opción alternativa: cargar datos sin navegar
  loadPublicProfile(userId: string, userService: UserService): void {
    userService.getPublicProfile(userId).subscribe({
      next: (profile) => {
        console.log('Perfil público:', profile);
      },
      error: (error) => {
        console.error('Usuario no encontrado:', error);
      },
    });
  }
}
