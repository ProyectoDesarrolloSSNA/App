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
      const userName = params['userName'];
      if (userName) {
        this.loadPublicProfile(userName);
      }
    });
  }

  loadPublicProfile(userName: string): void {
    // Buscar usuario en localStorage por userName
    const registeredUsers = JSON.parse(localStorage.getItem('registeredUsers') || '[]');
    const userProfile = registeredUsers.find((u: any) => u.userName === userName);

    if (userProfile) {
      // Cargar perfil guardado del usuario si existe
      const savedProfile = localStorage.getItem(`userProfile_${userName}`);
      let profileData = userProfile;

      if (savedProfile) {
        try {
          const parsedProfile = JSON.parse(savedProfile);
          profileData = { ...userProfile, ...parsedProfile };
        } catch (e) {
          console.error('Error parsing user profile:', e);
        }
      }

      this.profile = {
        id: userName,
        userName: profileData.userName,
        email: profileData.email,
        name: profileData.name || 'Usuario',
        surname: profileData.surname || '',
        phoneNumber: profileData.phoneNumber || '',
        profilePictureUrl: profileData.profilePictureUrl || null,
        bio: profileData.bio || '',
        createdAt: new Date(profileData.createdAt || Date.now()),
      };
      this.isLoading = false;
      this.error = null;
    } else {
      // Usuario no encontrado
      this.profile = null;
      this.isLoading = false;
      this.error = `No se encontró el usuario: ${userName}`;
    }
  }

  goBack(): void {
    window.history.back();
  }
}
