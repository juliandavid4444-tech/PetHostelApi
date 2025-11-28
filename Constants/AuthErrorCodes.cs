namespace PetHostelApi.Constants
{
    /// <summary>
    /// Códigos de error estandarizados para autenticación.
    /// Estos códigos deben ser usados en la app móvil para mostrar mensajes localizados.
    /// </summary>
    public static class AuthErrorCodes
    {
        // Success
        public const string SUCCESS = "AUTH_SUCCESS";

        // Validation errors
        public const string EMPTY_EMAIL = "AUTH_EMPTY_EMAIL";
        public const string EMPTY_PASSWORD = "AUTH_EMPTY_PASSWORD";
        public const string INVALID_EMAIL_FORMAT = "AUTH_INVALID_EMAIL_FORMAT";
        public const string INVALID_PASSWORD_FORMAT = "AUTH_INVALID_PASSWORD_FORMAT";
        public const string VALIDATION_ERROR = "AUTH_VALIDATION_ERROR";
        public const string INVALID_REQUEST = "AUTH_INVALID_REQUEST";

        // Authentication errors
        public const string USER_NOT_FOUND = "AUTH_USER_NOT_FOUND";
        public const string INCORRECT_PASSWORD = "AUTH_INCORRECT_PASSWORD";
        public const string INVALID_CREDENTIALS = "AUTH_INVALID_CREDENTIALS";
        public const string ACCOUNT_LOCKED = "AUTH_ACCOUNT_LOCKED";

        // Registration errors
        public const string EMAIL_ALREADY_EXISTS = "AUTH_EMAIL_ALREADY_EXISTS";
        public const string REGISTRATION_FAILED = "AUTH_REGISTRATION_FAILED";
        public const string WEAK_PASSWORD = "AUTH_WEAK_PASSWORD";

        // Token errors
        public const string INVALID_TOKEN = "AUTH_INVALID_TOKEN";
        public const string EXPIRED_TOKEN = "AUTH_EXPIRED_TOKEN";
        public const string TOKENS_REQUIRED = "AUTH_TOKENS_REQUIRED";

        // Server errors
        public const string SERVER_ERROR = "AUTH_SERVER_ERROR";
    }
}
