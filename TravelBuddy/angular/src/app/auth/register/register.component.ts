import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LocalAuthService } from '../auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
  registerForm: FormGroup;
  isLoading = false;
  error: string | null = null;
  successMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: LocalAuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group(
      {
        userName: ['', [Validators.required, Validators.minLength(3)]],
        email: ['', [Validators.required, Validators.email]],
        name: ['', [Validators.required, Validators.minLength(2)]],
        surname: ['', [Validators.required, Validators.minLength(2)]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required],
      },
      { validators: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator(group: AbstractControl): { [key: string]: any } | null {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { passwordMismatch: true };
  }

  register(): void {
    if (this.registerForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.error = null;
    this.successMessage = null;

    const { userName, email, name, surname, password } = this.registerForm.value;

    this.authService.register(userName, email, password, name, surname).subscribe({
      next: () => {
        this.successMessage = 'Cuenta creada exitosamente. Redirigiendo...';
        setTimeout(() => {
          this.router.navigate(['/users/profile']);
        }, 1500);
      },
      error: () => {
        this.error = 'Error al registrarse. Intenta de nuevo.';
        this.isLoading = false;
      },
    });
  }
}
