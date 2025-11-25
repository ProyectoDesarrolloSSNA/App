import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss'],
})
export class ChangePasswordComponent {
  changePasswordForm: FormGroup;
  isSaving = false;
  error: string | null = null;
  successMessage: string | null = null;
  showPassword = false;
  showNewPassword = false;
  showConfirmPassword = false;

  constructor(private fb: FormBuilder, private userService: UserService, private router: Router) {
    this.changePasswordForm = this.fb.group(
      {
        currentPassword: ['', Validators.required],
        newPassword: ['', [Validators.required, Validators.minLength(8)]],
        confirmPassword: ['', Validators.required],
      },
      { validators: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator(group: AbstractControl): { [key: string]: any } | null {
    const newPassword = group.get('newPassword')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return newPassword === confirmPassword ? null : { passwordMismatch: true };
  }

  changePassword(): void {
    if (this.changePasswordForm.invalid) {
      return;
    }

    this.isSaving = true;
    this.error = null;
    this.successMessage = null;

    // Simular cambio de contraseña mientras el backend no esté disponible
    setTimeout(() => {
      this.successMessage = 'Contraseña cambiada exitosamente (Demo)';
      this.changePasswordForm.reset();
      setTimeout(() => this.router.navigate(['/users/profile']), 2000);
    }, 1000);
  }

  goBack(): void {
    this.router.navigate(['/users/profile']);
  }

  togglePasswordVisibility(field: string): void {
    if (field === 'current') {
      this.showPassword = !this.showPassword;
    } else if (field === 'new') {
      this.showNewPassword = !this.showNewPassword;
    } else if (field === 'confirm') {
      this.showConfirmPassword = !this.showConfirmPassword;
    }
  }
}
