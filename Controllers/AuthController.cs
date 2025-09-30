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
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new MobileApiResponse<object>
                    {
                        Success = false,
                        Code = AuthErrorCodes.EMPTY_USERNAME
                    });
                }

                var result = _authService.Authenticate(request.Email, request.Password);

                return Ok(new MobileApiResponse<User>
                {
                    Success = result.IsSuccess,
                    Data = result.User,
                    Code = result.ErrorCode
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication");
                return StatusCode(500, new MobileApiResponse<object>
                {
                    Success = false,
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

