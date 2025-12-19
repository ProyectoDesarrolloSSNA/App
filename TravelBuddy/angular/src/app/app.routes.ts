import { authGuard, permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';
import { localAuthGuard } from './auth/auth.guard';

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'auth/login',
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(c => c.createRoutes()),
  },
  {
    path: 'identity',
    loadChildren: () => import('@abp/ng.identity').then(c => c.createRoutes()),
  },
  {
    path: 'tenant-management',
    loadChildren: () => import('@abp/ng.tenant-management').then(c => c.createRoutes()),
  },
  {
    path: 'setting-management',
    loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
  },
  {
    path: 'cities',
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./cities/search-city/search-city').then(m => m.SearchCityComponent),
      },
      {
        path: 'favorites',
        loadComponent: () =>
          import('./cities/saved-cities/saved-cities.component').then(c => c.SavedCitiesComponent),
        canActivate: [localAuthGuard],
      },
    ],
  },
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        loadComponent: () =>
          import('./auth/login/login.component').then(c => c.LoginComponent),
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./auth/register/register.component').then(c => c.RegisterComponent),
      },
    ],
  },
  {
    path: 'users',
    children: [
      {
        path: 'profile',
        loadComponent: () =>
          import('./users/profile/profile.component').then(c => c.ProfileComponent),
        canActivate: [localAuthGuard],
      },
      {
        path: 'edit-profile',
        loadComponent: () =>
          import('./users/edit-profile/edit-profile.component').then(c => c.EditProfileComponent),
        canActivate: [localAuthGuard],
      },
      {
        path: 'change-password',
        loadComponent: () =>
          import('./users/change-password/change-password.component').then(c => c.ChangePasswordComponent),
        canActivate: [localAuthGuard],
      },
      {
        path: 'public-profile/:userName',
        loadComponent: () =>
          import('./users/public-profile/public-profile.component').then(c => c.PublicProfileComponent),
      },
    ],
  },
];
