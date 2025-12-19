import { Injectable, inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { LocalAuthService } from './auth.service';

export const localAuthGuard: CanActivateFn = (route, state) => {
  const authService = inject(LocalAuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true;
  }

  router.navigate(['/auth/login']);
  return false;
};
