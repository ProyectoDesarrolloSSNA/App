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
    // Obtener usuario actual
    const currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
    
    // Cargar el perfil específico de este usuario usando su userName como clave
    const userProfileKey = `userProfile_${currentUser.userName}`;
    const savedProfile = localStorage.getItem(userProfileKey);

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
        localStorage.setItem(userProfileKey, JSON.stringify(profile));
        this.isLoading = false;
      },
      error: () => {
        // Crear perfil vacío para el usuario actual
        this.profile = {
          id: currentUser.id || 'user-new',
          userName: currentUser.userName || 'usuario',
          email: currentUser.email || 'usuario@example.com',
          name: currentUser.name || 'Usuario',
          surname: currentUser.surname || 'Demo',
          phoneNumber: '',
          profilePictureUrl: null,
          bio: '',
          createdAt: new Date(),
        };
        this.editForm.patchValue({
          name: this.profile.name,
          surname: this.profile.surname,
          email: this.profile.email,
          phoneNumber: this.profile.phoneNumber,
          bio: this.profile.bio,
        });
        localStorage.setItem(userProfileKey, JSON.stringify(this.profile));
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
    // No validar si el formulario es inválido, permitir guardar con datos parciales
    if (!this.profile) {
      return;
    }

    this.isSaving = true;
    this.error = null;
    this.successMessage = null;

    // Obtener usuario actual
    const currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
    const userProfileKey = `userProfile_${currentUser.userName}`;

    // Obtener valores del formulario (con fallback a valores actuales)
    const formValues = this.editForm.value;

    const updatedProfile: UserProfile = {
      ...this.profile,
      name: formValues.name?.trim() || this.profile.name,
      surname: formValues.surname?.trim() || this.profile.surname,
      email: formValues.email?.trim() || this.profile.email,
      phoneNumber: formValues.phoneNumber?.trim() || this.profile.phoneNumber || '',
      bio: formValues.bio?.trim() || this.profile.bio || '',
      // Incluir la foto si fue seleccionada
      profilePictureUrl: this.previewUrl || this.profile.profilePictureUrl,
    };

    console.log('Guardando perfil:', updatedProfile);

    // Simular actualización mientras el backend no está disponible
    setTimeout(() => {
      // Guardar en localStorage
      const currentUserData = JSON.parse(localStorage.getItem('currentUser') || '{}');
      const updatedUser = {
        ...currentUserData,
        name: updatedProfile.name,
        surname: updatedProfile.surname,
        email: updatedProfile.email,
      };
      localStorage.setItem('currentUser', JSON.stringify(updatedUser));

      // Crear una copia del perfil para guardar (con foto si existe)
      const profileToSave = { ...updatedProfile };
      
      console.log('Guardando en localStorage con clave:', userProfileKey);

      // Guardar perfil completo en localStorage con clave específica del usuario
      try {
        localStorage.setItem(userProfileKey, JSON.stringify(profileToSave));
      } catch (e) {
        // Si hay error por tamaño, guardar sin la foto muy grande
        console.warn('Foto muy grande, guardando sin foto');
        profileToSave.profilePictureUrl = null;
        localStorage.setItem(userProfileKey, JSON.stringify(profileToSave));
      }

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
