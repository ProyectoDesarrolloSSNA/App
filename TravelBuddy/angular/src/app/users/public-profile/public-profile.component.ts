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
    this.userService.getPublicProfile(userId).subscribe({
      next: (profile) => {
        this.profile = profile;
        this.isLoading = false;
      },
      error: () => {
        // Usar datos mock mientras el backend no esté disponible
        this.profile = {
          id: userId,
          userName: 'maria.garcia',
          email: 'maria.garcia@example.com',
          name: 'María',
          surname: 'García',
          phoneNumber: '+34 623 456 789',
          profilePictureUrl: null,
          bio: 'Exploradora de lugares exóticos. Fotógrafa de viajes.',
          createdAt: new Date('2023-06-20'),
        };
        this.isLoading = false;
        this.error = null;
      },
    });
  }

  goBack(): void {
    window.history.back();
  }
}
