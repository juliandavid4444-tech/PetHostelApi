# PetHostel API Documentation

## Overview
PetHostel API is a RESTful API built with ASP.NET Core 7.0 for managing pet hostel services. The API includes JWT-based authentication with refresh tokens using ASP.NET Identity.

### Key Features
- 🔐 JWT Authentication with Access & Refresh Tokens
- 👤 ASP.NET Identity for user management
- 🌍 Internationalization-ready (returns error codes for client-side translation)
- 📱 Optimized for mobile applications
- 🗄️ Azure SQL Database
- 📝 Swagger/OpenAPI documentation

### Technology Stack
- **Framework:** ASP.NET Core 7.0
- **Authentication:** JWT Bearer + ASP.NET Identity
- **Database:** Azure SQL Server
- **ORM:** Entity Framework Core 7.0
- **API Documentation:** Swagger/Swashbuckle

---

## Authentication System

The PetHostel API returns standardized error codes instead of localized messages. Client applications should implement their own translations based on these codes.

## Response Format

### Success Response
```json
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "VgIvUlB7...",
  "accessTokenExpiration": "2025-11-28T00:08:36Z",
  "refreshTokenExpiration": "2025-12-04T23:53:36Z",
  "user": {
    "id": "c89a9caa-b143-4a6e-987e-e5d4427c7b9d",
    "email": "test@pethostel.com",
    "firstName": "Test",
    "lastName": "User",
    "fullName": "Test User"
  }
}
```

### Error Response
```json
{
  "success": false,
  "code": "AUTH_EMPTY_EMAIL",
  "data": null,
  "parameters": null
}
```

## Error Codes Reference

### Validation Errors (400 Bad Request)

| Code | Description | When It Occurs |
|------|-------------|----------------|
| `AUTH_EMPTY_EMAIL` | Email field is empty or null | User didn't provide email in request |
| `AUTH_EMPTY_PASSWORD` | Password field is empty or null | User didn't provide password in request |
| `AUTH_INVALID_EMAIL_FORMAT` | Email format is invalid | Email doesn't match standard email pattern (e.g., "user@domain.com") |
| `AUTH_INVALID_PASSWORD_FORMAT` | Password doesn't meet requirements | Password is too weak or doesn't meet minimum criteria |
| `AUTH_VALIDATION_ERROR` | General validation error | Required fields like firstName or lastName are missing |
| `AUTH_INVALID_REQUEST` | Request structure is invalid | Malformed JSON or missing required fields |

### Authentication Errors (401 Unauthorized)

| Code | Description | When It Occurs |
|------|-------------|----------------|
| `AUTH_INVALID_CREDENTIALS` | Email or password is incorrect | User provided wrong email or password during login |
| `AUTH_USER_NOT_FOUND` | User doesn't exist in system | No user found with provided email |
| `AUTH_INCORRECT_PASSWORD` | Password is wrong | Email exists but password doesn't match |
| `AUTH_ACCOUNT_LOCKED` | User account is locked | Account has been disabled or temporarily locked |

### Registration Errors (400 Bad Request)

| Code | Description | When It Occurs |
|------|-------------|----------------|
| `AUTH_EMAIL_ALREADY_EXISTS` | Email is already registered | User tries to register with an email that's already in use |
| `AUTH_REGISTRATION_FAILED` | Registration process failed | General registration error (check server logs) |
| `AUTH_WEAK_PASSWORD` | Password doesn't meet security requirements | Password is too short or lacks complexity |

### Token Errors (400 Bad Request / 401 Unauthorized)

| Code | Description | When It Occurs |
|------|-------------|----------------|
| `AUTH_TOKENS_REQUIRED` | Access token or refresh token missing | Client didn't provide required tokens |
| `AUTH_INVALID_TOKEN` | Token is invalid or expired | Token signature is invalid, token has expired, or token has been revoked |
| `AUTH_EXPIRED_TOKEN` | Token has expired | Token expiration time has passed |

### Server Errors (500 Internal Server Error)

| Code | Description | When It Occurs |
|------|-------------|----------------|
| `AUTH_SERVER_ERROR` | Internal server error | Unexpected server error (check server logs) |

### Success Code

| Code | Description | When It Occurs |
|------|-------------|----------------|
| `AUTH_SUCCESS` | Operation successful | Authentication or registration completed successfully |

## API Endpoints

### POST /api/authentication/register
Register a new user.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "YourPassword123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Possible Error Codes:**
- `AUTH_EMPTY_EMAIL`
- `AUTH_EMPTY_PASSWORD`
- `AUTH_INVALID_EMAIL_FORMAT`
- `AUTH_VALIDATION_ERROR`
- `AUTH_EMAIL_ALREADY_EXISTS`
- `AUTH_WEAK_PASSWORD`
- `AUTH_SERVER_ERROR`

---

### POST /api/authentication/login
Authenticate user and get tokens.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "YourPassword123!"
}
```

**Possible Error Codes:**
- `AUTH_EMPTY_EMAIL`
- `AUTH_EMPTY_PASSWORD`
- `AUTH_INVALID_EMAIL_FORMAT`
- `AUTH_INVALID_CREDENTIALS`
- `AUTH_USER_NOT_FOUND`
- `AUTH_INCORRECT_PASSWORD`
- `AUTH_ACCOUNT_LOCKED`
- `AUTH_SERVER_ERROR`

---

### POST /api/authentication/refresh
Refresh access token using refresh token.

**Request Body:**
```json
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "VgIvUlB7..."
}
```

**Possible Error Codes:**
- `AUTH_TOKENS_REQUIRED`
- `AUTH_INVALID_TOKEN`
- `AUTH_EXPIRED_TOKEN`
- `AUTH_SERVER_ERROR`

---

### POST /api/authentication/revoke
Revoke a specific refresh token (requires authentication).

**Request Body:**
```json
{
  "refreshToken": "VgIvUlB7..."
}
```

**Possible Error Codes:**
- `AUTH_TOKENS_REQUIRED`
- `AUTH_INVALID_TOKEN`
- `AUTH_SERVER_ERROR`

---

### POST /api/authentication/revoke-all
Revoke all refresh tokens for current user (requires authentication).

**Possible Error Codes:**
- `AUTH_SERVER_ERROR`

---

### GET /api/authentication/me
Get current authenticated user information (requires authentication).

**Possible Error Codes:**
- `AUTH_INVALID_TOKEN`
- `AUTH_EXPIRED_TOKEN`
- `AUTH_SERVER_ERROR`

---

## Client Implementation Guidelines

### 1. Error Code Constants
Create enums or constants in your client application:

**Android (Kotlin):**
```kotlin
enum class AuthErrorCode(val code: String) {
    SUCCESS("AUTH_SUCCESS"),
    EMPTY_EMAIL("AUTH_EMPTY_EMAIL"),
    EMPTY_PASSWORD("AUTH_EMPTY_PASSWORD"),
    INVALID_EMAIL_FORMAT("AUTH_INVALID_EMAIL_FORMAT"),
    INVALID_CREDENTIALS("AUTH_INVALID_CREDENTIALS"),
    EMAIL_ALREADY_EXISTS("AUTH_EMAIL_ALREADY_EXISTS"),
    INVALID_TOKEN("AUTH_INVALID_TOKEN"),
    SERVER_ERROR("AUTH_SERVER_ERROR")
    // ... add all codes
}
```

**iOS (Swift):**
```swift
enum AuthErrorCode: String {
    case success = "AUTH_SUCCESS"
    case emptyEmail = "AUTH_EMPTY_EMAIL"
    case emptyPassword = "AUTH_EMPTY_PASSWORD"
    case invalidEmailFormat = "AUTH_INVALID_EMAIL_FORMAT"
    case invalidCredentials = "AUTH_INVALID_CREDENTIALS"
    case emailAlreadyExists = "AUTH_EMAIL_ALREADY_EXISTS"
    case invalidToken = "AUTH_INVALID_TOKEN"
    case serverError = "AUTH_SERVER_ERROR"
    // ... add all codes
}
```

### 2. Localized Strings
Implement localized strings for each error code:

**Android (strings.xml):**
```xml
<!-- English -->
<string name="auth_empty_email">Email is required</string>
<string name="auth_empty_password">Password is required</string>
<string name="auth_invalid_credentials">Invalid email or password</string>

<!-- Spanish (values-es/strings.xml) -->
<string name="auth_empty_email">El email es requerido</string>
<string name="auth_empty_password">La contraseña es requerida</string>
<string name="auth_invalid_credentials">Email o contraseña incorrectos</string>
```

**iOS (Localizable.strings):**
```
// English
"auth_empty_email" = "Email is required";
"auth_empty_password" = "Password is required";
"auth_invalid_credentials" = "Invalid email or password";

// Spanish (es.lproj/Localizable.strings)
"auth_empty_email" = "El email es requerido";
"auth_empty_password" = "La contraseña es requerida";
"auth_invalid_credentials" = "Email o contraseña incorrectos";
```

### 3. Error Handling
Map error codes to localized messages:

**Android (Kotlin):**
```kotlin
fun getErrorMessage(errorCode: String, context: Context): String {
    return when (errorCode) {
        "AUTH_EMPTY_EMAIL" -> context.getString(R.string.auth_empty_email)
        "AUTH_EMPTY_PASSWORD" -> context.getString(R.string.auth_empty_password)
        "AUTH_INVALID_CREDENTIALS" -> context.getString(R.string.auth_invalid_credentials)
        else -> context.getString(R.string.auth_unknown_error)
    }
}
```

**iOS (Swift):**
```swift
func getErrorMessage(errorCode: String) -> String {
    switch errorCode {
    case "AUTH_EMPTY_EMAIL":
        return NSLocalizedString("auth_empty_email", comment: "")
    case "AUTH_EMPTY_PASSWORD":
        return NSLocalizedString("auth_empty_password", comment: "")
    case "AUTH_INVALID_CREDENTIALS":
        return NSLocalizedString("auth_invalid_credentials", comment: "")
    default:
        return NSLocalizedString("auth_unknown_error", comment: "")
    }
}
```

## Token Management

### Access Token
- **Lifetime:** 15 minutes
- **Usage:** Include in Authorization header: `Authorization: Bearer {accessToken}`
- **When expired:** Use refresh token to get new access token

### Refresh Token
- **Lifetime:** 7 days
- **Usage:** Send to `/api/authentication/refresh` endpoint
- **Storage:** Store securely (iOS Keychain, Android EncryptedSharedPreferences)
- **When expired:** User must login again

## Best Practices

1. **Always validate on client side first** before sending requests
2. **Store tokens securely** using platform-specific secure storage
3. **Implement automatic token refresh** before access token expires
4. **Handle 401 errors** by refreshing token or redirecting to login
5. **Log out on token revocation** and clear stored tokens
6. **Implement retry logic** for network errors
7. **Don't expose error details** to end users for security reasons

## Example Client Flow

```
1. User enters email and password
2. Client validates format locally
3. If valid, send POST /api/authentication/login
4. On success:
   - Store accessToken and refreshToken securely
   - Navigate to main screen
5. On error:
   - Parse error code
   - Show localized error message
   - Let user retry

6. For authenticated requests:
   - Include: Authorization: Bearer {accessToken}
   - If 401 error: Try refresh token
   - If refresh fails: Redirect to login

7. Before access token expires (e.g., 1 min before):
   - Automatically call POST /api/authentication/refresh
   - Update stored tokens
```

## Support
For questions or issues, please contact the development team.
