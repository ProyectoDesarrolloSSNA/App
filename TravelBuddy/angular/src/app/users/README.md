# Gestión de Usuarios - Documentación Frontend

## Descripción General

Este módulo implementa las funcionalidades completas de gestión de usuarios en el frontend Angular, incluyendo:

1. **Registro de nuevos usuarios** - Manejado por `@abp/ng.account`
2. **Iniciar sesión** - Manejado por `@abp/ng.account`
3. **Actualizar datos de perfil** - Componente `EditProfileComponent`
4. **Cambiar contraseña** - Componente `ChangePasswordComponent`
5. **Eliminar la propia cuenta** - Funcionalidad en `ProfileComponent`
6. **Consultar perfil público** - Componente `PublicProfileComponent`

## Estructura de Directorios

```
src/app/users/
├── services/
│   ├── user.service.ts          # Servicio principal de usuarios
│   ├── user.models.ts           # Interfaces y DTOs
│   └── user.service.spec.ts     # Tests del servicio
├── profile/
│   ├── profile.component.ts     # Ver perfil propio
│   ├── profile.component.html
│   ├── profile.component.scss
│   └── profile.component.spec.ts
├── edit-profile/
│   ├── edit-profile.component.ts     # Editar perfil y foto
│   ├── edit-profile.component.html
│   ├── edit-profile.component.scss
│   └── edit-profile.component.spec.ts
├── change-password/
│   ├── change-password.component.ts  # Cambiar contraseña
│   ├── change-password.component.html
│   ├── change-password.component.scss
│   └── change-password.component.spec.ts
└── public-profile/
    ├── public-profile.component.ts   # Ver perfil de otros usuarios
    ├── public-profile.component.html
    ├── public-profile.component.scss
    └── public-profile.component.spec.ts
```

## Servicios

### UserService

El servicio principal que maneja todas las operaciones relacionadas con usuarios.

#### Métodos Disponibles

```typescript
// Obtener perfil actual
getCurrentProfile(): Observable<UserProfile>

// Actualizar perfil del usuario
updateProfile(userId: string, profile: Partial<UserProfile>): Observable<UserProfile>

// Subir foto de perfil
uploadProfilePicture(userId: string, file: File): Observable<{ url: string }>

// Actualizar preferencias del usuario
updatePreferences(userId: string, preferences: UserPreferences): Observable<UserProfile>

// Cambiar contraseña
changePassword(request: ChangePasswordRequest): Observable<void>

// Eliminar cuenta
deleteAccount(userId: string): Observable<void>

// Obtener perfil público
getPublicProfile(userId: string): Observable<UserProfile>
```

## Componentes

### 1. ProfileComponent

Muestra el perfil del usuario autenticado con opciones para editar, cambiar contraseña o eliminar la cuenta.

**Ruta:** `/users/profile`  
**Protección:** `authGuard`

**Funcionalidades:**
- Carga automática del perfil actual
- Muestra de información del usuario (nombre, email, bio, foto)
- Acceso rápido a opciones de edición
- Opción para eliminar la cuenta

### 2. EditProfileComponent

Permite actualizar los datos del perfil incluyendo información personal y foto de perfil.

**Ruta:** `/users/edit-profile`  
**Protección:** `authGuard`

**Funcionalidades:**
- Edición de nombre, apellido, email y teléfono
- Edición de biografía con límite de caracteres
- Carga y vista previa de foto de perfil
- Validación de formulario en tiempo real
- Mensajes de éxito y error

**Validaciones:**
- Nombre y apellido: mínimo 2 caracteres
- Email: formato válido de email
- Biografía: máximo 500 caracteres
- Foto: máximo 5MB, formatos JPG/PNG/GIF

### 3. ChangePasswordComponent

Permite cambiar la contraseña del usuario con validaciones de seguridad.

**Ruta:** `/users/change-password`  
**Protección:** `authGuard`

**Funcionalidades:**
- Solicitud de contraseña actual para verificación
- Ingreso de nueva contraseña con confirmación
- Toggle para mostrar/ocultar contraseña
- Validaciones de seguridad
- Consejos de seguridad integrados

**Validaciones:**
- Contraseña actual: requerida
- Nueva contraseña: mínimo 8 caracteres
- Confirmación: debe coincidir con nueva contraseña

### 4. PublicProfileComponent

Muestra el perfil público de otros usuarios sin capacidad de edición.

**Ruta:** `/users/public/:id`  
**Protección:** Ninguna (accesible para todos)

**Funcionalidades:**
- Visualización de perfil público
- Muestra de información del usuario (nombre, bio, fecha de unión)
- Manejo de usuarios no encontrados

## Rutas

Las rutas están configuradas en `src/app/app.routes.ts`:

```typescript
{
  path: 'users',
  children: [
    {
      path: 'profile',
      loadComponent: () => import('./users/profile/profile.component').then(c => c.ProfileComponent),
      canActivate: [authGuard],
    },
    {
      path: 'edit-profile',
      loadComponent: () => import('./users/edit-profile/edit-profile.component').then(c => c.EditProfileComponent),
      canActivate: [authGuard],
    },
    {
      path: 'change-password',
      loadComponent: () => import('./users/change-password/change-password.component').then(c => c.ChangePasswordComponent),
      canActivate: [authGuard],
    },
    {
      path: 'public/:id',
      loadComponent: () => import('./users/public-profile/public-profile.component').then(c => c.PublicProfileComponent),
    },
  ],
}
```

## Configuración del Backend

El servicio espera los siguientes endpoints en tu API:

```
GET    /api/app/users/me                    # Obtener perfil actual
GET    /api/app/users/:id                   # Obtener perfil por ID
GET    /api/app/users/:id/public            # Obtener perfil público
PUT    /api/app/users/:id                   # Actualizar perfil
POST   /api/app/users/:id/profile-picture   # Subir foto de perfil
PUT    /api/app/users/:id/preferences       # Actualizar preferencias
POST   /api/app/users/change-password       # Cambiar contraseña
DELETE /api/app/users/:id                   # Eliminar cuenta
```

## Interfaz de Datos

### UserProfile

```typescript
interface UserProfile {
  id: string;
  userName: string;
  email: string;
  name: string;
  surname: string;
  phoneNumber?: string;
  profilePictureUrl?: string;
  bio?: string;
  preferences?: UserPreferences;
  createdAt: Date;
}
```

### UserPreferences

```typescript
interface UserPreferences {
  theme: 'light' | 'dark';
  notifications: boolean;
  language: string;
}
```

### ChangePasswordRequest

```typescript
interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}
```

## Estilos

Los componentes utilizan SCSS con:
- Diseño responsive
- Validación visual de formularios
- Animaciones suaves
- Paleta de colores consistente con Bootstrap

**Variables de Color:**
- Primario: `#007bff` (Azul)
- Peligro: `#dc3545` (Rojo)
- Secundario: `#6c757d` (Gris)
- Éxito: `#28a745` (Verde)
- Advertencia: `#ffc107` (Amarillo)

## Autenticación

Todos los componentes están protegidos con `authGuard` de ABP, excepto el perfil público. Los usuarios deben estar autenticados para acceder a sus propios perfiles.

## Manejo de Errores

Cada componente incluye:
- Mensajes de error claros y específicos
- Estados de carga
- Validación de formularios en tiempo real
- Manejo de excepciones HTTP

## Testing

Se incluyen tests unitarios básicos para cada componente usando Jasmine/Karma:

```bash
ng test
```

Los tests cubren:
- Creación de componentes
- Carga de datos
- Validación de formularios
- Manejo de errores
- Llamadas a servicios

## Uso en la Aplicación

### Navegar al perfil del usuario actual

```typescript
this.router.navigate(['/users/profile']);
```

### Navegar al perfil público de otro usuario

```typescript
this.router.navigate(['/users/public', userId]);
```

### Usar el servicio en otros componentes

```typescript
import { UserService } from './users/services/user.service';

constructor(private userService: UserService) {}

ngOnInit() {
  this.userService.getCurrentProfile().subscribe(profile => {
    console.log(profile);
  });
}
```

## Notas Importantes

1. **Carga Lazy Loading**: Los componentes se cargan de forma perezosa (lazy loading) para mejorar el rendimiento inicial de la aplicación.

2. **Seguridad**: Las contraseñas se envían directamente al servidor HTTPS. Nunca se almacenan en localStorage.

3. **Foto de Perfil**: Las fotos se cargan como FormData. Asegúrate de que tu API las procese correctamente.

4. **Validación**: La mayoría de las validaciones ocurren en el cliente, pero siempre valida también en el servidor.

5. **Estado**: Los datos se cargan directamente cuando se accede a los componentes. Considera implementar un estado global (NgRx, Akita) si necesitas compartir datos entre múltiples componentes.

## Mejoras Futuras

- [ ] Implementar caché de perfiles
- [ ] Agregar paginación a historial de actividad
- [ ] Integrar verificación de email
- [ ] Autenticación de dos factores
- [ ] Historial de cambios de contraseña
- [ ] Exportar datos del usuario
- [ ] Roles y permisos avanzados
- [ ] Integración con redes sociales
