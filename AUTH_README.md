# API de Autenticación - Manejo de Errores

## Nuevas Características de Autenticación

### Respuestas de Error Mejoradas

La API ahora proporciona mensajes de error específicos y códigos de estado HTTP apropiados para diferentes tipos de errores de autenticación.

### Tipos de Errores

| Error Type | HTTP Status | Error Code | Descripción |
|------------|-------------|------------|-------------|
| `ValidationError` | 400 | `VALIDATION_ERROR` | Datos de entrada faltantes o inválidos |
| `UserNotFound` | 401 | `USER_NOT_FOUND` | El usuario no existe |
| `IncorrectPassword` | 401 | `INCORRECT_PASSWORD` | Contraseña incorrecta |
| `InvalidCredentials` | 401 | `INVALID_CREDENTIALS` | Error general de credenciales |
| `AccountLocked` | 423 | `ACCOUNT_LOCKED` | Cuenta bloqueada |

### Ejemplos de Respuestas

#### Login Exitoso
```json
{
  "success": true,
  "data": {
    "id": 1,
    "user_user": "john_doe",
    "user_email": "john@example.com"
  },
  "message": "Login exitoso."
}
```

#### Error: Usuario no encontrado
```json
{
  "message": "Usuario o contraseña incorrectos.",
  "errorCode": "USER_NOT_FOUND",
  "details": "El usuario proporcionado no existe en el sistema.",
  "timestamp": "2023-09-27T10:00:00Z"
}
```

#### Error: Contraseña incorrecta
```json
{
  "message": "Usuario o contraseña incorrectos.",
  "errorCode": "INCORRECT_PASSWORD",
  "details": "La contraseña proporcionada es incorrecta.",
  "timestamp": "2023-09-27T10:00:00Z"
}
```

#### Error: Validación
```json
{
  "message": "El nombre de usuario es requerido.",
  "errorCode": "VALIDATION_ERROR",
  "details": "Los datos proporcionados no cumplen con los requisitos.",
  "timestamp": "2023-09-27T10:00:00Z"
}
```

### Uso desde el Cliente

```javascript
// Ejemplo de manejo de errores en JavaScript
async function login(username, password) {
    try {
        const response = await fetch('/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                user: username,
                password: password
            })
        });

        const data = await response.json();

        if (response.ok) {
            // Login exitoso
            console.log('Usuario logueado:', data.data);
            return { success: true, user: data.data };
        } else {
            // Error de autenticación
            switch (data.errorCode) {
                case 'USER_NOT_FOUND':
                    return { success: false, error: 'El usuario no existe' };
                case 'INCORRECT_PASSWORD':
                    return { success: false, error: 'Contraseña incorrecta' };
                case 'VALIDATION_ERROR':
                    return { success: false, error: data.message };
                default:
                    return { success: false, error: 'Error de autenticación' };
            }
        }
    } catch (error) {
        console.error('Error de conexión:', error);
        return { success: false, error: 'Error de conexión con el servidor' };
    }
}
```

### Logs de Seguridad

El sistema ahora registra intentos de login fallidos para análisis de seguridad:

- ✅ Login exitoso se registra con nivel `Information`
- ⚠️ Usuario no encontrado se registra con nivel `Warning`
- ⚠️ Contraseña incorrecta se registra con nivel `Warning`
- ❌ Errores del sistema se registran con nivel `Error`

### Consideraciones de Seguridad

1. **Mensajes genéricos**: Los errores de usuario no encontrado y contraseña incorrecta devuelven el mismo mensaje para evitar ataques de enumeración de usuarios.

2. **Logging**: Todos los intentos de login fallidos se registran para monitoreo de seguridad.

3. **Rate Limiting**: Se recomienda implementar rate limiting para prevenir ataques de fuerza bruta.

4. **HTTPS**: Siempre usar HTTPS en producción para proteger las credenciales en tránsito.