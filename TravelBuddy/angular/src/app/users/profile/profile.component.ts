import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { UserService, UserProfile } from '../services/user.service';
import { LocalAuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent implements OnInit {
  profile: UserProfile | null = null;
  isLoading = true;
  error: string | null = null;
  isDeletingAccount = false;

  constructor(
    private userService: UserService,
    private authService: LocalAuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    // Obtener usuario actual
    const currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
    
    // Cargar el perfil específico de este usuario usando su userName como clave
    const userProfileKey = `userProfile_${currentUser.userName}`;
    const savedProfile = localStorage.getItem(userProfileKey);
    
    if (savedProfile) {
      this.profile = JSON.parse(savedProfile);
      this.isLoading = false;
      return;
    }

    this.userService.getCurrentProfile().subscribe({
      next: (profile) => {
        this.profile = profile;
        localStorage.setItem(userProfileKey, JSON.stringify(profile));
        this.isLoading = false;
      },
      error: () => {
        // Crear perfil vacío para el usuario actual
        const mockProfile: UserProfile = {
          id: currentUser.id || '123e4567-e89b-12d3-a456-426614174000',
          userName: currentUser.userName || 'usuario',
          email: currentUser.email || 'usuario@example.com',
          name: currentUser.name || 'Usuario',
          surname: currentUser.surname || 'Demo',
          phoneNumber: '',
          profilePictureUrl: null,
          bio: '',
          createdAt: new Date(),
        };
        this.profile = mockProfile;
        localStorage.setItem(userProfileKey, JSON.stringify(mockProfile));
        this.isLoading = false;
        this.error = null;
      },
    });
  }

  navigateToEdit(): void {
    this.router.navigate(['/users/edit-profile']);
  }

  navigateToChangePassword(): void {
    this.router.navigate(['/users/change-password']);
  }

  deleteAccount(): void {
    if (confirm('¿Estás seguro de que deseas eliminar tu cuenta? Esta acción no se puede deshacer.')) {
      this.isDeletingAccount = true;
      
      // Usar el servicio para eliminar la cuenta
      this.authService.deleteAccount(this.profile!.id, this.profile!.userName).subscribe({
        next: () => {
          alert('Cuenta eliminada exitosamente');
          this.router.navigate(['/auth/login']);
        },
        error: () => {
          this.error = 'Error al eliminar la cuenta';
          this.isDeletingAccount = false;
        },
      });
    }
  }
}
