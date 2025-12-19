# Configuración de Backend para Gestión de Usuarios

## Endpoints Requeridos

El frontend espera los siguientes endpoints en tu API backend:

### 1. Obtener Perfil Actual del Usuario
```
GET /api/app/users/me
Authorization: Bearer {token}

Respuesta (200 OK):
{
  "id": "string (UUID)",
  "userName": "string",
  "email": "string",
  "name": "string",
  "surname": "string",
  "phoneNumber": "string (opcional)",
  "profilePictureUrl": "string (opcional, URL)",
  "bio": "string (opcional)",
  "preferences": {
    "theme": "light" | "dark",
    "notifications": boolean,
    "language": "string"
  },
  "createdAt": "ISO 8601 DateTime"
}
```

### 2. Obtener Perfil por ID
```
GET /api/app/users/{userId}
Authorization: Bearer {token}

Respuesta (200 OK):
{
  "id": "string (UUID)",
  "userName": "string",
  "email": "string",
  "name": "string",
  "surname": "string",
  "createdAt": "ISO 8601 DateTime"
}

Errores:
- 404 Not Found: Si el usuario no existe
- 401 Unauthorized: Si no está autenticado
```

### 3. Obtener Perfil Público
```
GET /api/app/users/{userId}/public

Respuesta (200 OK):
{
  "id": "string (UUID)",
  "userName": "string",
  "name": "string",
  "surname": "string",
  "profilePictureUrl": "string (opcional)",
  "bio": "string (opcional)",
  "createdAt": "ISO 8601 DateTime"
}

Nota: No retorna email ni información sensible
Errores:
- 404 Not Found: Si el usuario no existe
```

### 4. Actualizar Perfil
```
PUT /api/app/users/{userId}
Authorization: Bearer {token}
Content-Type: application/json

Body:
{
  "name": "string (requerido)",
  "surname": "string (requerido)",
  "email": "string (requerido, email válido)",
  "phoneNumber": "string (opcional)",
  "bio": "string (opcional, máximo 500 caracteres)"
}

Respuesta (200 OK):
{
  "id": "string",
  "userName": "string",
  "email": "string",
  "name": "string",
  "surname": "string",
  "phoneNumber": "string",
  "profilePictureUrl": "string",
  "bio": "string",
  "preferences": { ... },
  "createdAt": "ISO 8601 DateTime"
}

Validaciones:
- Email debe ser único en la base de datos
- Nombre y apellido deben tener mínimo 2 caracteres
- Bio máximo 500 caracteres

Errores:
- 400 Bad Request: Validación fallida
- 401 Unauthorized: No está autenticado
- 403 Forbidden: No puede editar este perfil
- 409 Conflict: Email ya existe
```

### 5. Subir Foto de Perfil
```
POST /api/app/users/{userId}/profile-picture
Authorization: Bearer {token}
Content-Type: multipart/form-data

Form Data:
- file: File (JPG, PNG, GIF, máximo 5MB)

Respuesta (200 OK):
{
  "url": "string (URL de la imagen guardada)"
}

Validaciones:
- Tipo de archivo: image/jpeg, image/png, image/gif
- Tamaño máximo: 5MB
- Debe ser una imagen válida

Errores:
- 400 Bad Request: Archivo inválido
- 401 Unauthorized: No está autenticado
- 413 Payload Too Large: Archivo demasiado grande
```

### 6. Actualizar Preferencias
```
PUT /api/app/users/{userId}/preferences
Authorization: Bearer {token}
Content-Type: application/json

Body:
{
  "theme": "light" | "dark",
  "notifications": boolean,
  "language": "en" | "es" | ... (códigos de idioma ISO 639-1)
}

Respuesta (200 OK):
{
  "id": "string",
  "userName": "string",
  "email": "string",
  "name": "string",
  "surname": "string",
  "preferences": {
    "theme": "light" | "dark",
    "notifications": boolean,
    "language": "string"
  },
  "createdAt": "ISO 8601 DateTime"
}

Errores:
- 400 Bad Request: Validación fallida
- 401 Unauthorized: No está autenticado
```

### 7. Cambiar Contraseña
```
POST /api/app/users/change-password
Authorization: Bearer {token}
Content-Type: application/json

Body:
{
  "currentPassword": "string (requerida)",
  "newPassword": "string (requerida, mínimo 8 caracteres)",
  "confirmPassword": "string (debe coincidir con newPassword)"
}

Respuesta (200 OK):
{}

Validaciones:
- Contraseña actual debe ser correcta
- Nueva contraseña mínimo 8 caracteres
- Confirmación debe coincidir
- No puede ser la misma contraseña actual

Errores:
- 400 Bad Request: Validación fallida o contraseña actual incorrecta
- 401 Unauthorized: No está autenticado
```

### 8. Eliminar Cuenta
```
DELETE /api/app/users/{userId}
Authorization: Bearer {token}

Respuesta (200 OK):
{}

Consideraciones:
- Requiere confirmación adicional de seguridad
- Debe invalidar todos los tokens del usuario
- Puede soft-delete o hard-delete según política
- Considerar guardar datos anónimos

Errores:
- 401 Unauthorized: No está autenticado
- 403 Forbidden: No puede eliminar este usuario
```

## Implementación Recomendada (C# / .NET)

### AppUserManager (Application Service)

```csharp
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TravelBuddy.Users
{
    public class UserAppService : ApplicationService
    {
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IdentityUserManager _userManager;
        private readonly IUserProfileService _profileService;

        public UserAppService(
            IRepository<AppUser, Guid> userRepository,
            IdentityUserManager userManager,
            IUserProfileService profileService)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _profileService = profileService;
        }

        // Obtener perfil actual
        [Authorize]
        public virtual async Task<UserProfileDto> GetMyProfileAsync()
        {
            var user = await _userManager.GetUserAsync(CurrentUser.GetId());
            return ObjectMapper.Map<AppUser, UserProfileDto>(user);
        }

        // Obtener perfil por ID
        [Authorize]
        public virtual async Task<UserProfileDto> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
            return ObjectMapper.Map<AppUser, UserProfileDto>(user);
        }

        // Obtener perfil público
        public virtual async Task<PublicUserProfileDto> GetPublicProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
            return ObjectMapper.Map<AppUser, PublicUserProfileDto>(user);
        }

        // Actualizar perfil
        [Authorize]
        public virtual async Task<UserProfileDto> UpdateProfileAsync(UpdateUserProfileDto input)
        {
            var user = await _userManager.GetUserAsync(CurrentUser.GetId());
            
            user.Name = input.Name;
            user.Surname = input.Surname;
            user.PhoneNumber = input.PhoneNumber;
            user.Bio = input.Bio;

            if (input.Email != user.Email)
            {
                user.Email = input.Email;
                user.NormalizedEmail = input.Email.ToUpper();
            }

            await _userManager.UpdateAsync(user);
            return ObjectMapper.Map<AppUser, UserProfileDto>(user);
        }

        // Subir foto de perfil
        [Authorize]
        public virtual async Task<UploadProfilePictureResultDto> UploadProfilePictureAsync(
            Guid userId, IRemoteStreamContent file)
        {
            // Implementar lógica de almacenamiento
            var url = await _profileService.SaveProfilePictureAsync(userId, file);
            return new UploadProfilePictureResultDto { Url = url };
        }

        // Cambiar contraseña
        [Authorize]
        public virtual async Task ChangePasswordAsync(ChangePasswordDto input)
        {
            var user = await _userManager.GetUserAsync(CurrentUser.GetId());
            
            var result = await _userManager.ChangePasswordAsync(
                user, 
                input.CurrentPassword, 
                input.NewPassword);

            if (!result.Succeeded)
            {
                throw new UserFriendlyException(
                    L["PasswordChangeFailed"], 
                    string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }

        // Eliminar cuenta
        [Authorize]
        public virtual async Task DeleteAccountAsync()
        {
            var user = await _userManager.GetUserAsync(CurrentUser.GetId());
            await _userManager.DeleteAsync(user);
        }
    }
}
```

### DTOs

```csharp
using System;

namespace TravelBuddy.Users
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Bio { get; set; }
        public UserPreferencesDto Preferences { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PublicUserProfileDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Bio { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateUserProfileDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Bio { get; set; }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class UserPreferencesDto
    {
        public string Theme { get; set; }
        public bool Notifications { get; set; }
        public string Language { get; set; }
    }
}
```

## Seguridad

### Consideraciones Importantes:

1. **Autenticación**: Todos los endpoints (excepto perfil público) requieren token JWT válido
2. **Autorización**: Los usuarios solo pueden editar su propio perfil
3. **Email único**: Validar que el email sea único antes de actualizar
4. **Foto de perfil**: Validar tipo, tamaño y escanear por malware
5. **Contraseña**: Usar bcrypt o algoritmo seguro similar
6. **HTTPS**: Obligatorio para todas las comunicaciones
7. **CORS**: Configurar correctamente para el dominio frontend
8. **Rate Limiting**: Implementar para endpoints sensibles
9. **Logging**: Registrar cambios sensibles (cambio de email, contraseña, eliminación)
10. **Backup**: Mantener copias de seguridad antes de eliminar cuentas

## Testing

### Tests de Unidad Recomendados:

- Validación de email único
- Validación de longitud de contraseña
- Validación de tipo de archivo de imagen
- Validación de tamaño de archivo
- Permisos de acceso (usuario solo puede editar el suyo)
- Manejo de errores y excepciones

## Consideraciones de Performance

1. Cachear perfiles públicos (frecuentemente consultados)
2. Usar índices en userName y email
3. Implementar paginación si se necesita listar usuarios
4. Considerar usar stored procedures para operaciones complejas
