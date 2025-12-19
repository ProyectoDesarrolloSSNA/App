import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LocalAuthService } from '../auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: LocalAuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      userName: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  login(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.error = null;

    const { userName, password } = this.loginForm.value;

    this.authService.login(userName, password).subscribe({
      next: () => {
        this.router.navigate(['/cities']);
      },
      error: (err) => {
        if (err.type === 'USER_NOT_FOUND') {
          this.error = 'El usuario no existe. Por favor regístrate.';
        } else if (err.type === 'INVALID_PASSWORD') {
          this.error = 'Contraseña incorrecta. Intenta de nuevo.';
        } else {
          this.error = err.message || 'Error al iniciar sesión';
        }
        this.isLoading = false;
      },
    });
  }
}
