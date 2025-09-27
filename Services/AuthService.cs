using System;
using PetHostelApi.Entities;
using PetHostelApi.Contexts;

namespace PetHostelApi.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AppDbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public AuthResult Authenticate(string userName, string password)
        {
            // Validación de entrada
            if (string.IsNullOrWhiteSpace(userName))
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorType = AuthErrorType.EmptyUsername,
                    ErrorCode = AuthErrorCodes.EMPTY_USERNAME
                };
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorType = AuthErrorType.EmptyPassword,
                    ErrorCode = AuthErrorCodes.EMPTY_PASSWORD
                };
            }

            try
            {
                // Buscar el usuario por nombre de usuario
                var user = _context.User.FirstOrDefault(u => u.user_user == userName);
                
                if (user == null)
                {
                    _logger.LogWarning("Intento de login fallido: Usuario '{UserName}' no encontrado", userName);
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorType = AuthErrorType.UserNotFound,
                        ErrorCode = AuthErrorCodes.USER_NOT_FOUND,
                        ErrorParameters = new Dictionary<string, object> { { "username", userName } }
                    };
                }

                // Verificar la contraseña
                if (user.user_password != password)
                {
                    _logger.LogWarning("Intento de login fallido: Contraseña incorrecta para usuario '{UserName}'", userName);
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorType = AuthErrorType.IncorrectPassword,
                        ErrorCode = AuthErrorCodes.INCORRECT_PASSWORD,
                        ErrorParameters = new Dictionary<string, object> { { "username", userName } }
                    };
                }

                _logger.LogInformation("Login exitoso para usuario '{UserName}'", userName);
                return new AuthResult
                {
                    IsSuccess = true,
                    User = user,
                    ErrorType = AuthErrorType.None,
                    ErrorCode = AuthErrorCodes.SUCCESS
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la autenticación del usuario '{UserName}'", userName);
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorType = AuthErrorType.InvalidCredentials,
                    ErrorCode = AuthErrorCodes.SERVER_ERROR
                };
            }
        }

        // Método para validar credenciales sin exponer información sensible
        public bool ValidateCredentials(string userName, string password)
        {
            var result = Authenticate(userName, password);
            return result.IsSuccess;
        }
    }
}

