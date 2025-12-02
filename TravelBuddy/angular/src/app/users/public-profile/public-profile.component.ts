import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService, UserProfile } from '../services/user.service';

@Component({
  selector: 'app-public-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './public-profile.component.html',
  styleUrls: ['./public-profile.component.scss'],
})
export class PublicProfileComponent implements OnInit {
  profile: UserProfile | null = null;
  isLoading = true;
  error: string | null = null;

  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      const userId = params['id'];
      if (userId) {
        this.loadPublicProfile(userId);
      }
    });
  }

  loadPublicProfile(userId: string): void {
    // Simular búsqueda de usuario
    // En una app real, esto llamaría al backend
    
    this.userService.getPublicProfile(userId).subscribe({
      next: (profile) => {
        this.profile = profile;
        this.isLoading = false;
      },
      error: () => {
        // Verificar si es el usuario actual o mostrar error
        const currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
        
        // Si el ID coincide con el usuario actual, mostrar su perfil
        if (userId === currentUser.id || userId === '123e4567-e89b-12d3-a456-426614174000') {
          this.profile = {
            id: userId,
            userName: currentUser.userName || 'maria.garcia',
            email: currentUser.email || 'maria.garcia@example.com',
            name: currentUser.name || 'María',
            surname: currentUser.surname || 'García',
            phoneNumber: '+34 623 456 789',
            profilePictureUrl: null,
            bio: 'Exploradora de lugares exóticos. Fotógrafa de viajes.',
            createdAt: new Date('2023-06-20'),
          };
          this.isLoading = false;
          this.error = null;
        } else {
          // Usuario no encontrado
          this.profile = null;
          this.isLoading = false;
          this.error = `No se encontró el usuario con ID: ${userId}`;
        }
      },
    });
  }

  goBack(): void {
    window.history.back();
  }
}
