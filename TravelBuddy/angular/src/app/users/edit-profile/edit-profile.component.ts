import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService, UserProfile } from '../services/user.service';

@Component({
  selector: 'app-edit-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.scss'],
})
export class EditProfileComponent implements OnInit {
  editForm: FormGroup;
  profile: UserProfile | null = null;
  isLoading = true;
  isSaving = false;
  error: string | null = null;
  successMessage: string | null = null;
  selectedFile: File | null = null;
  previewUrl: string | null = null;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router
  ) {
    this.editForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      surname: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: [''],
      bio: ['', Validators.maxLength(500)],
    });
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    // Primero, intentar cargar del localStorage
    const savedProfile = localStorage.getItem('userProfile');
    const currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');

    if (savedProfile) {
      const storedProfile = JSON.parse(savedProfile);
      this.profile = storedProfile;
      this.previewUrl = storedProfile.profilePictureUrl || null;
      this.editForm.patchValue({
        name: storedProfile.name,
        surname: storedProfile.surname,
        email: storedProfile.email,
        phoneNumber: storedProfile.phoneNumber,
        bio: storedProfile.bio,
      });
      this.isLoading = false;
      return;
    }

    this.userService.getCurrentProfile().subscribe({
      next: (profile) => {
        this.profile = profile;
        this.previewUrl = profile.profilePictureUrl || null;
        this.editForm.patchValue({
          name: profile.name,
          surname: profile.surname,
          email: profile.email,
          phoneNumber: profile.phoneNumber,
          bio: profile.bio,
        });
        localStorage.setItem('userProfile', JSON.stringify(profile));
        this.isLoading = false;
      },
      error: () => {
        // Usar datos mock mientras el backend no esté disponible
        this.profile = {
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
        this.editForm.patchValue({
          name: this.profile.name,
          surname: this.profile.surname,
          email: this.profile.email,
          phoneNumber: this.profile.phoneNumber,
          bio: this.profile.bio,
        });
        localStorage.setItem('userProfile', JSON.stringify(this.profile));
        this.isLoading = false;
      },
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      // Validar que sea una imagen
      if (!file.type.startsWith('image/')) {
        this.error = 'Por favor selecciona un archivo de imagen válido';
        return;
      }

      // Validar tamaño (máximo 5MB)
      if (file.size > 5 * 1024 * 1024) {
        this.error = 'La imagen no debe ser mayor a 5MB';
        return;
      }

      this.selectedFile = file;
      
      // Mostrar preview
      const reader = new FileReader();
      reader.onload = (e) => {
        this.previewUrl = e.target?.result as string;
      };
      reader.readAsDataURL(file);
      this.error = null;
    }
  }

  saveProfile(): void {
    if (this.editForm.invalid || !this.profile) {
      return;
    }

    this.isSaving = true;
    this.error = null;
    this.successMessage = null;

    const updatedProfile: UserProfile = {
      ...this.profile,
      name: this.editForm.get('name')?.value,
      surname: this.editForm.get('surname')?.value,
      email: this.editForm.get('email')?.value,
      phoneNumber: this.editForm.get('phoneNumber')?.value,
      bio: this.editForm.get('bio')?.value,
      // Incluir la foto si fue seleccionada
      profilePictureUrl: this.previewUrl || this.profile.profilePictureUrl,
    };

    // Simular actualización mientras el backend no está disponible
    setTimeout(() => {
      // Guardar en localStorage
      const currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
      const updatedUser = {
        ...currentUser,
        name: updatedProfile.name,
        surname: updatedProfile.surname,
        email: updatedProfile.email,
      };
      localStorage.setItem('currentUser', JSON.stringify(updatedUser));

      // Guardar perfil con la foto en localStorage
      localStorage.setItem('userProfile', JSON.stringify(updatedProfile));

      this.profile = updatedProfile;
      this.isSaving = false;
      this.successMessage = 'Perfil actualizado exitosamente ✓';
      
      // Redirigir después de 2 segundos
      setTimeout(() => {
        this.router.navigate(['/users/profile']);
      }, 2000);
    }, 1000);
  }

  goBack(): void {
    this.router.navigate(['/users/profile']);
  }
}
