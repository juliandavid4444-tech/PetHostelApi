using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PetHostelApi.Entities;
using PetHostelApi.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PetHostelApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly AuthService _authService;

        public AuthController(ILogger<AuthController> logger, AuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("login")]
        public ActionResult<MobileApiResponse<User>> Login(LoginRequest request)
        {
            try
            {
                // Validación adicional del request
                if (request == null)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Code = AuthErrorCodes.INVALID_REQUEST
                    });
                }

                var authResult = _authService.Authenticate(request.user, request.password);
                
                if (!authResult.IsSuccess)
                {
                    var statusCode = GetStatusCode(authResult.ErrorType);
                    
                    var errorResponse = new ErrorResponse
                    {
                        Code = authResult.ErrorCode,
                        Parameters = authResult.ErrorParameters
                    };

                    return StatusCode(statusCode, errorResponse);
                }

                // Login exitoso
                var successResponse = new MobileApiResponse<User>
                {
                    Success = true,
                    Data = authResult.User,
                    Code = AuthErrorCodes.SUCCESS
                };

                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado durante el proceso de login");
                
                return StatusCode(500, new ErrorResponse
                {
                    Code = AuthErrorCodes.SERVER_ERROR
                });
            }
        }

        private int GetStatusCode(AuthErrorType errorType)
        {
            return errorType switch
            {
                AuthErrorType.EmptyUsername => 400,      // Bad Request
                AuthErrorType.EmptyPassword => 400,      // Bad Request
                AuthErrorType.ValidationError => 400,    // Bad Request
                AuthErrorType.UserNotFound => 401,       // Unauthorized
                AuthErrorType.IncorrectPassword => 401,  // Unauthorized
                AuthErrorType.AccountLocked => 423,      // Locked
                AuthErrorType.InvalidCredentials => 401, // Unauthorized
                _ => 401 // Unauthorized por defecto
            };
        }
    }
}

