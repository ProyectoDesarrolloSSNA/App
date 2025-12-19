# 🚀 Guía Rápida de Inicio - Gestión de Usuarios

## ✨ Lo que se ha implementado

Se ha creado un módulo completo de gestión de usuarios con 4 componentes principales:

1. **ProfileComponent** - Ver perfil personal
2. **EditProfileComponent** - Editar datos personales y foto
3. **ChangePasswordComponent** - Cambiar contraseña
4. **PublicProfileComponent** - Ver perfiles públicos de otros usuarios

## 📦 Requisitos

- Angular 20+
- ABP 9.3.2+
- Node.js 18+
- npm o yarn

## 🛠️ Instalación

### 1. Dependencias ya incluidas
No necesitas instalar nada adicional. Todo está usando:
- `@angular/forms` (Reactive Forms)
- `@angular/common`
- `@abp/ng.core`
- `rxjs`

### 2. Verificar la instalación

```bash
npm install
```

### 3. Compilar el proyecto

```bash
ng build
```

O para desarrollo:

```bash
ng serve
```

## 🔌 Configuración Necesaria

### 1. Verificar variables de entorno

Archivo: `src/environments/environment.ts`

```typescript
export const environment = {
  apis: {
    default: {
      url: 'https://localhost:44367',  // ← Asegúrate que esta URL sea correcta
      rootNamespace: 'TravelBuddy',
    },
  },
};
```

### 2. Asegurar que las rutas estén correctas

Archivo: `src/app/app.routes.ts` ✅ **Ya configurado**

## 🧪 Prueba los componentes

### 1. En desarrollo

```bash
ng serve
```

Abre: `http://localhost:4200`

### 2. Navegar a los componentes

- Perfil: http://localhost:4200/users/profile
- Editar: http://localhost:4200/users/edit-profile
- Cambiar contraseña: http://localhost:4200/users/change-password
- Perfil público: http://localhost:4200/users/public/[userId]

## 📋 Checklist de Funciones

### Frontend ✅

- [x] Componente de perfil
- [x] Componente de edición de perfil
- [x] Componente de cambio de contraseña
- [x] Componente de perfil público
- [x] Servicio de usuarios
- [x] Validaciones de formulario
- [x] Estilos responsive
- [x] Manejo de errores
- [x] Tests unitarios

### Backend 🔲 - **PENDIENTE**

Necesitas implementar en tu API:

```
GET    /api/app/users/me
GET    /api/app/users/:id/public
PUT    /api/app/users/:id
POST   /api/app/users/:id/profile-picture
POST   /api/app/users/change-password
DELETE /api/app/users/:id
```

Ver: `BACKEND_SETUP.md` para detalles completos

## 🔑 API Requerida - Resumen Rápido

```
Método   Endpoint                          Autenticación  Descripción
------   --------                          ----------------  -----------
GET      /api/app/users/me                 ✅ Requerida    Perfil actual
GET      /api/app/users/{id}/public        ❌ Opcional     Perfil público
GET      /api/app/users/{id}               ✅ Requerida    Perfil por ID
PUT      /api/app/users/{id}               ✅ Requerida    Actualizar perfil
POST     /api/app/users/{id}/profile-pic   ✅ Requerida    Subir foto
POST     /api/app/users/change-password    ✅ Requerida    Cambiar pass
DELETE   /api/app/users/{id}               ✅ Requerida    Eliminar cuenta
```

## 🎯 Paso 1: Usar en tu aplicación

### Agregar link a perfil en navbar

En tu componente de navbar (típicamente en `app.component.ts` o layout):

```typescript
import { RouterLink } from '@angular/router';

@Component({
  template: `
    <nav>
      <a routerLink="/users/profile">Mi Perfil</a>
      <a routerLink="/users/edit-profile">Editar</a>
    </nav>
  `,
  imports: [RouterLink]
})
export class NavbarComponent {}
```

### Usar el servicio en otro componente

```typescript
import { Component, OnInit } from '@angular/core';
import { UserService } from './users/services/user.service';

@Component({
  selector: 'app-example',
  template: `<div>{{ userName }}</div>`
})
export class ExampleComponent implements OnInit {
  userName: string = '';

  constructor(private userService: UserService) {}

  ngOnInit() {
    this.userService.getCurrentProfile().subscribe({
      next: (profile) => {
        this.userName = profile.name;
      },
      error: (error) => {
        console.error('Error:', error);
      }
    });
  }
}
```

## 🔧 Paso 2: Implementar Backend (C# - ABP)

### Crear Application Service

```csharp
[RemoteService]
[Area("app")]
[ControllerName("Users")]
[Route("api/app/users")]
public class UserController : AbpController
{
    private readonly IUserAppService _userAppService;

    public UserController(IUserAppService userAppService)
    {
        _userAppService = userAppService;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<UserProfileDto> GetMe()
    {
        return await _userAppService.GetMyProfileAsync();
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<UserProfileDto> UpdateProfile(Guid id, UpdateUserProfileDto input)
    {
        return await _userAppService.UpdateProfileAsync(id, input);
    }

    [HttpPost("{id}/profile-picture")]
    [Authorize]
    public async Task<UploadProfilePictureResultDto> UploadProfilePicture(
        Guid id, IFormFile file)
    {
        return await _userAppService.UploadProfilePictureAsync(id, file);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task ChangePassword(ChangePasswordDto input)
    {
        await _userAppService.ChangePasswordAsync(input);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task DeleteAccount(Guid id)
    {
        await _userAppService.DeleteAccountAsync(id);
    }

    [HttpGet("{id}/public")]
    public async Task<PublicUserProfileDto> GetPublicProfile(Guid id)
    {
        return await _userAppService.GetPublicProfileAsync(id);
    }
}
```

## 🧩 Paso 3: Mapeos (AutoMapper)

En tu módulo de aplicación, configurar mapeos:

```csharp
CreateMap<AppUser, UserProfileDto>();
CreateMap<AppUser, PublicUserProfileDto>();
CreateMap<UpdateUserProfileDto, AppUser>();
```

## 📊 Estructura de Carpetas Final

```
TravelBuddy/angular/
├── src/
│   ├── app/
│   │   ├── users/                    ← NUEVO
│   │   │   ├── services/
│   │   │   │   ├── user.service.ts
│   │   │   │   └── user.models.ts
│   │   │   ├── profile/
│   │   │   ├── edit-profile/
│   │   │   ├── change-password/
│   │   │   ├── public-profile/
│   │   │   ├── examples/
│   │   │   └── README.md
│   │   ├── app.routes.ts             ← ACTUALIZADO
│   │   └── ...otros componentes
│   └── ...
├── IMPLEMENTATION_SUMMARY.md         ← NUEVO
├── BACKEND_SETUP.md                  ← NUEVO
└── ...
```

## ✅ Validaciones Implementadas

### EditProfileComponent
- ✅ Nombre: mínimo 2 caracteres
- ✅ Email: formato válido
- ✅ Bio: máximo 500 caracteres
- ✅ Foto: máximo 5MB, JPG/PNG/GIF

### ChangePasswordComponent
- ✅ Contraseña actual: requerida
- ✅ Nueva contraseña: mínimo 8 caracteres
- ✅ Coincidencia de contraseñas

## 🐛 Troubleshooting

### Error: "Cannot find module"
```bash
npm install
ng serve
```

### Error 404 en API
Verifica que:
1. La API está corriendo: `https://localhost:44367`
2. El endpoint existe: `GET /api/app/users/me`
3. El token JWT es válido

### Error CORS
Configurar CORS en el backend:
```csharp
context.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
        builder
            .WithOrigins("http://localhost:4200", "https://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
```

### Foto no se sube
Verificar:
1. El endpoint existe: `POST /api/app/users/{id}/profile-picture`
2. Acepta `multipart/form-data`
3. El archivo tiene menos de 5MB

## 📚 Documentación Completa

- **Documentación del módulo:** `src/app/users/README.md`
- **Setup del backend:** `BACKEND_SETUP.md`
- **Resumen de implementación:** `IMPLEMENTATION_SUMMARY.md`

## 🎓 Ejemplos de Uso

Ver archivo: `src/app/users/examples/integration.example.ts`

Incluye ejemplos de:
- Menú de usuario
- Acceso al perfil
- Navegación a perfiles públicos

## ✨ Siguiente Paso Recomendado

1. ✅ Frontend implementado
2. 🔲 **Implementar endpoints backend** ← AHORA
3. 🔲 Probar integración
4. 🔲 Agregar tests E2E
5. 🔲 Considerar mejoras

---

**¿Necesitas ayuda?** Revisa la documentación en los archivos `.md` incluidos o contacta al equipo.
