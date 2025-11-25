import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService, LocalizationPipe } from '@abp/ng.core';
import { LocalAuthService } from '../auth/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: [LocalizationPipe]
})
export class HomeComponent implements OnInit {
  private abpAuthService = inject(AuthService);
  private localAuthService = inject(LocalAuthService);
  private router = inject(Router);

  get hasLoggedIn(): boolean {
    return this.localAuthService.isAuthenticated();
  }

  ngOnInit() {
    // Redirigir a login si no está autenticado
    if (!this.hasLoggedIn) {
      this.router.navigate(['/auth/login']);
    }
  }

  login() {
    this.router.navigate(['/auth/login']);
  }
}
