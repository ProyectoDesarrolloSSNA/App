import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { UserService, UserProfile } from '../services/user.service';

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

  constructor(private userService: UserService, private router: Router) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    // Primero, intentar cargar del localStorage
    const savedProfile = localStorage.getItem('userProfile');
    const currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
    
    if (savedProfile) {
      this.profile = JSON.parse(savedProfile);
      this.isLoading = false;
      return;
    }

    this.userService.getCurrentProfile().subscribe({
      next: (profile) => {
        this.profile = profile;
        localStorage.setItem('userProfile', JSON.stringify(profile));
        this.isLoading = false;
      },
      error: () => {
        // Usar datos mock mientras el backend no esté disponible
        const mockProfile: UserProfile = {
          id: '123e4567-e89b-12d3-a456-426614174000',
          userName: currentUser.userName || 'juan.perez',
          email: currentUser.email || 'juan.perez@example.com',
          name: currentUser.name || 'Juan',
          surname: currentUser.surname || 'Pérez',
          phoneNumber: '+34 612 345 678',
          profilePictureUrl: null,
          bio: 'Viajero apasionado por conocer nuevas culturas y destinos increíbles.',
          createdAt: new Date('2024-01-15'),
        };
        this.profile = mockProfile;
        localStorage.setItem('userProfile', JSON.stringify(mockProfile));
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
      this.userService.deleteAccount(this.profile!.id).subscribe({
        next: () => {
          alert('Cuenta eliminada exitosamente');
          this.router.navigate(['/']);
        },
        error: () => {
          this.error = 'Error al eliminar la cuenta';
        },
      });
    }
  }
}
