using System;

namespace PetHostelApi.Entities
{
    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public User? User { get; set; }
        public string ErrorCode { get; set; } = string.Empty;
        public AuthErrorType ErrorType { get; set; }
        public Dictionary<string, object>? ErrorParameters { get; set; }
    }

    public enum AuthErrorType
    {
        None,
        UserNotFound,
        IncorrectPassword,
        InvalidCredentials,
        AccountLocked,
        ValidationError,
        EmptyUsername,
        EmptyPassword,
        InvalidFormat
    }

    // Códigos de error consistentes para el cliente
    public static class AuthErrorCodes
    {
        public const string SUCCESS = "AUTH_SUCCESS";
        public const string USER_NOT_FOUND = "AUTH_USER_NOT_FOUND";
        public const string INCORRECT_PASSWORD = "AUTH_INCORRECT_PASSWORD";
        public const string INVALID_CREDENTIALS = "AUTH_INVALID_CREDENTIALS";
        public const string ACCOUNT_LOCKED = "AUTH_ACCOUNT_LOCKED";
        public const string VALIDATION_ERROR = "AUTH_VALIDATION_ERROR";
        public const string EMPTY_USERNAME = "AUTH_EMPTY_USERNAME";
        public const string EMPTY_PASSWORD = "AUTH_EMPTY_PASSWORD";
        public const string INVALID_FORMAT = "AUTH_INVALID_FORMAT";
        public const string SERVER_ERROR = "AUTH_SERVER_ERROR";
        public const string INVALID_REQUEST = "AUTH_INVALID_REQUEST";
    }
}