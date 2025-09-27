# Manejo de Errores de Autenticación para Apps Móviles

## Estrategia: Códigos de Error + Internacionalización en Cliente

### ¿Por qué códigos de error?

✅ **Ventajas:**
- **Performance**: Backend no procesa idiomas
- **Flexibilidad**: Cambias traducciones sin tocar el backend
- **Consistencia**: Un solo lugar para todas las traducciones
- **Soporte offline**: App muestra errores sin conexión
- **Personalización**: Diferente tono por plataforma (iOS/Android)

### Respuesta del API

```json
// ✅ Login Exitoso
{
  "success": true,
  "data": {
    "id": 1,
    "user_user": "john_doe",
    "user_email": "john@example.com"
  },
  "code": "AUTH_SUCCESS",
  "timestamp": 1695816000
}

// ❌ Error de Autenticación
{
  "success": false,
  "code": "AUTH_USER_NOT_FOUND",
  "params": {
    "username": "john_doe"
  },
  "timestamp": 1695816000
}
```

### Códigos de Error Disponibles

| Código | HTTP Status | Descripción |
|--------|-------------|-------------|
| `AUTH_SUCCESS` | 200 | Login exitoso |
| `AUTH_EMPTY_USERNAME` | 400 | Username vacío |
| `AUTH_EMPTY_PASSWORD` | 400 | Password vacío |
| `AUTH_USER_NOT_FOUND` | 401 | Usuario no existe |
| `AUTH_INCORRECT_PASSWORD` | 401 | Contraseña incorrecta |
| `AUTH_ACCOUNT_LOCKED` | 423 | Cuenta bloqueada |
| `AUTH_SERVER_ERROR` | 500 | Error interno |
| `AUTH_INVALID_REQUEST` | 400 | Request inválido |

## Implementación en Apps Móviles

### Android (Kotlin)

```kotlin
// 1. Crear enum para códigos de error
enum class AuthErrorCode(val code: String) {
    SUCCESS("AUTH_SUCCESS"),
    EMPTY_USERNAME("AUTH_EMPTY_USERNAME"),
    EMPTY_PASSWORD("AUTH_EMPTY_PASSWORD"),
    USER_NOT_FOUND("AUTH_USER_NOT_FOUND"),
    INCORRECT_PASSWORD("AUTH_INCORRECT_PASSWORD"),
    ACCOUNT_LOCKED("AUTH_ACCOUNT_LOCKED"),
    SERVER_ERROR("AUTH_SERVER_ERROR"),
    INVALID_REQUEST("AUTH_INVALID_REQUEST")
}

// 2. Crear archivo strings.xml para cada idioma

// res/values/strings.xml (Español)
<resources>
    <string name="auth_success">¡Bienvenido!</string>
    <string name="auth_empty_username">El nombre de usuario es requerido</string>
    <string name="auth_empty_password">La contraseña es requerida</string>
    <string name="auth_user_not_found">Usuario o contraseña incorrectos</string>
    <string name="auth_incorrect_password">Usuario o contraseña incorrectos</string>
    <string name="auth_account_locked">Tu cuenta está bloqueada. Contacta soporte.</string>
    <string name="auth_server_error">Error del servidor. Inténtalo más tarde.</string>
    <string name="auth_invalid_request">Datos inválidos. Verifica tu información.</string>
    <string name="auth_unknown_error">Error desconocido. Inténtalo de nuevo.</string>
</resources>

// res/values-en/strings.xml (English)
<resources>
    <string name="auth_success">Welcome!</string>
    <string name="auth_empty_username">Username is required</string>
    <string name="auth_empty_password">Password is required</string>
    <string name="auth_user_not_found">Invalid username or password</string>
    <string name="auth_incorrect_password">Invalid username or password</string>
    <string name="auth_account_locked">Your account is locked. Contact support.</string>
    <string name="auth_server_error">Server error. Please try again later.</string>
    <string name="auth_invalid_request">Invalid data. Please check your information.</string>
    <string name="auth_unknown_error">Unknown error. Please try again.</string>
</resources>

// 3. Clase para manejar respuestas de autenticación
data class AuthResponse(
    val success: Boolean,
    val data: User? = null,
    val code: String,
    val params: Map<String, Any>? = null,
    val timestamp: Long
)

class AuthErrorHandler(private val context: Context) {
    
    fun getErrorMessage(errorCode: String): String {
        return when (errorCode) {
            AuthErrorCode.SUCCESS.code -> context.getString(R.string.auth_success)
            AuthErrorCode.EMPTY_USERNAME.code -> context.getString(R.string.auth_empty_username)
            AuthErrorCode.EMPTY_PASSWORD.code -> context.getString(R.string.auth_empty_password)
            AuthErrorCode.USER_NOT_FOUND.code -> context.getString(R.string.auth_user_not_found)
            AuthErrorCode.INCORRECT_PASSWORD.code -> context.getString(R.string.auth_incorrect_password)
            AuthErrorCode.ACCOUNT_LOCKED.code -> context.getString(R.string.auth_account_locked)
            AuthErrorCode.SERVER_ERROR.code -> context.getString(R.string.auth_server_error)
            AuthErrorCode.INVALID_REQUEST.code -> context.getString(R.string.auth_invalid_request)
            else -> context.getString(R.string.auth_unknown_error)
        }
    }
}

// 4. Usar en tu Activity/Fragment
class LoginActivity : AppCompatActivity() {
    private val authErrorHandler = AuthErrorHandler(this)
    
    private fun handleAuthResponse(response: AuthResponse) {
        if (response.success) {
            // Login exitoso
            val message = authErrorHandler.getErrorMessage(response.code)
            Toast.makeText(this, message, Toast.LENGTH_SHORT).show()
            // Navegar a la siguiente pantalla
        } else {
            // Error de autenticación
            val errorMessage = authErrorHandler.getErrorMessage(response.code)
            showErrorDialog(errorMessage)
        }
    }
}
```

### iOS (Swift)

```swift
// 1. Enum para códigos de error
enum AuthErrorCode: String, CaseIterable {
    case success = "AUTH_SUCCESS"
    case emptyUsername = "AUTH_EMPTY_USERNAME"
    case emptyPassword = "AUTH_EMPTY_PASSWORD"
    case userNotFound = "AUTH_USER_NOT_FOUND"
    case incorrectPassword = "AUTH_INCORRECT_PASSWORD"
    case accountLocked = "AUTH_ACCOUNT_LOCKED"
    case serverError = "AUTH_SERVER_ERROR"
    case invalidRequest = "AUTH_INVALID_REQUEST"
}

// 2. Crear Localizable.strings para cada idioma

// Localizable.strings (Español)
"auth_success" = "¡Bienvenido!";
"auth_empty_username" = "El nombre de usuario es requerido";
"auth_empty_password" = "La contraseña es requerida";
"auth_user_not_found" = "Usuario o contraseña incorrectos";
"auth_incorrect_password" = "Usuario o contraseña incorrectos";
"auth_account_locked" = "Tu cuenta está bloqueada. Contacta soporte.";
"auth_server_error" = "Error del servidor. Inténtalo más tarde.";
"auth_invalid_request" = "Datos inválidos. Verifica tu información.";
"auth_unknown_error" = "Error desconocido. Inténtalo de nuevo.";

// en.lproj/Localizable.strings (English)
"auth_success" = "Welcome!";
"auth_empty_username" = "Username is required";
"auth_empty_password" = "Password is required";
"auth_user_not_found" = "Invalid username or password";
"auth_incorrect_password" = "Invalid username or password";
"auth_account_locked" = "Your account is locked. Contact support.";
"auth_server_error" = "Server error. Please try again later.";
"auth_invalid_request" = "Invalid data. Please check your information.";
"auth_unknown_error" = "Unknown error. Please try again.";

// 3. Modelos de respuesta
struct AuthResponse: Codable {
    let success: Bool
    let data: User?
    let code: String
    let params: [String: Any]?
    let timestamp: Int64
}

// 4. Manejador de errores
class AuthErrorHandler {
    
    static func getErrorMessage(for errorCode: String) -> String {
        switch AuthErrorCode(rawValue: errorCode) {
        case .success:
            return NSLocalizedString("auth_success", comment: "")
        case .emptyUsername:
            return NSLocalizedString("auth_empty_username", comment: "")
        case .emptyPassword:
            return NSLocalizedString("auth_empty_password", comment: "")
        case .userNotFound:
            return NSLocalizedString("auth_user_not_found", comment: "")
        case .incorrectPassword:
            return NSLocalizedString("auth_incorrect_password", comment: "")
        case .accountLocked:
            return NSLocalizedString("auth_account_locked", comment: "")
        case .serverError:
            return NSLocalizedString("auth_server_error", comment: "")
        case .invalidRequest:
            return NSLocalizedString("auth_invalid_request", comment: "")
        default:
            return NSLocalizedString("auth_unknown_error", comment: "")
        }
    }
}

// 5. Usar en tu ViewController
class LoginViewController: UIViewController {
    
    private func handleAuthResponse(_ response: AuthResponse) {
        let message = AuthErrorHandler.getErrorMessage(for: response.code)
        
        if response.success {
            // Login exitoso
            showSuccessMessage(message)
            // Navegar a la siguiente pantalla
        } else {
            // Error de autenticación
            showErrorAlert(title: "Error", message: message)
        }
    }
    
    private func showErrorAlert(title: String, message: String) {
        let alert = UIAlertController(title: title, message: message, preferredStyle: .alert)
        alert.addAction(UIAlertAction(title: "OK", style: .default))
        present(alert, animated: true)
    }
}
```

### React Native (JavaScript/TypeScript)

```javascript
// 1. Constantes de códigos de error
export const AuthErrorCodes = {
    SUCCESS: 'AUTH_SUCCESS',
    EMPTY_USERNAME: 'AUTH_EMPTY_USERNAME',
    EMPTY_PASSWORD: 'AUTH_EMPTY_PASSWORD',
    USER_NOT_FOUND: 'AUTH_USER_NOT_FOUND',
    INCORRECT_PASSWORD: 'AUTH_INCORRECT_PASSWORD',
    ACCOUNT_LOCKED: 'AUTH_ACCOUNT_LOCKED',
    SERVER_ERROR: 'AUTH_SERVER_ERROR',
    INVALID_REQUEST: 'AUTH_INVALID_REQUEST'
};

// 2. Configuración de i18n (usando react-i18next)
// locales/es.json
{
    "auth": {
        "success": "¡Bienvenido!",
        "empty_username": "El nombre de usuario es requerido",
        "empty_password": "La contraseña es requerida",
        "user_not_found": "Usuario o contraseña incorrectos",
        "incorrect_password": "Usuario o contraseña incorrectos",
        "account_locked": "Tu cuenta está bloqueada. Contacta soporte.",
        "server_error": "Error del servidor. Inténtalo más tarde.",
        "invalid_request": "Datos inválidos. Verifica tu información.",
        "unknown_error": "Error desconocido. Inténtalo de nuevo."
    }
}

// locales/en.json
{
    "auth": {
        "success": "Welcome!",
        "empty_username": "Username is required",
        "empty_password": "Password is required",
        "user_not_found": "Invalid username or password",
        "incorrect_password": "Invalid username or password",
        "account_locked": "Your account is locked. Contact support.",
        "server_error": "Server error. Please try again later.",
        "invalid_request": "Invalid data. Please check your information.",
        "unknown_error": "Unknown error. Please try again."
    }
}

// 3. Hook para manejar errores de autenticación
import { useTranslation } from 'react-i18next';

export const useAuthError = () => {
    const { t } = useTranslation();
    
    const getErrorMessage = (errorCode) => {
        switch (errorCode) {
            case AuthErrorCodes.SUCCESS:
                return t('auth.success');
            case AuthErrorCodes.EMPTY_USERNAME:
                return t('auth.empty_username');
            case AuthErrorCodes.EMPTY_PASSWORD:
                return t('auth.empty_password');
            case AuthErrorCodes.USER_NOT_FOUND:
                return t('auth.user_not_found');
            case AuthErrorCodes.INCORRECT_PASSWORD:
                return t('auth.incorrect_password');
            case AuthErrorCodes.ACCOUNT_LOCKED:
                return t('auth.account_locked');
            case AuthErrorCodes.SERVER_ERROR:
                return t('auth.server_error');
            case AuthErrorCodes.INVALID_REQUEST:
                return t('auth.invalid_request');
            default:
                return t('auth.unknown_error');
        }
    };
    
    return { getErrorMessage };
};

// 4. Usar en componente
import React from 'react';
import { Alert } from 'react-native';
import { useAuthError } from './hooks/useAuthError';

const LoginScreen = () => {
    const { getErrorMessage } = useAuthError();
    
    const handleAuthResponse = (response) => {
        const message = getErrorMessage(response.code);
        
        if (response.success) {
            // Login exitoso
            Alert.alert('Éxito', message);
            // Navegar a la siguiente pantalla
        } else {
            // Error de autenticación
            Alert.alert('Error', message);
        }
    };
    
    return (
        // Tu componente de login
    );
};
```

## Ventajas de este Enfoque

1. **🚀 Performance**: El backend no tiene que procesar idiomas
2. **🔧 Mantenimiento**: Un solo lugar para cada plataforma
3. **📱 Experiencia de usuario**: Mensajes consistentes en el idioma correcto
4. **⚡ Offline**: Los mensajes funcionan sin conexión
5. **🎨 Personalización**: Diferentes tonos/estilos por plataforma
6. **🔍 Debug**: Fácil identificar errores específicos

## Consejos Adicionales

- Usa **códigos descriptivos** pero no demasiado largos
- Mantén **consistencia** en el naming (`AUTH_`, `USER_`, etc.)
- Implementa **logging** en el backend para análisis
- Considera **rate limiting** para prevenir ataques
- Usa **HTTPS** siempre en producción