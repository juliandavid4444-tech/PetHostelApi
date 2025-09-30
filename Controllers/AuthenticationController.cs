using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetHostelApi.Entities;
using PetHostelApi.Services;

namespace PetHostelApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IJwtService jwtService, ILogger<AuthenticationController> logger)
        {
            _jwtService = jwtService;
            _logger = logger;
        }

        /// <summary>
        /// Registrar un nuevo usuario
        /// </summary>
        /// <param name="request">Datos del usuario a registrar</param>
        /// <returns>Tokens de autenticación y datos del usuario</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Code = "INVALID_REGISTRATION_DATA",
                        Data = ModelState.Where(x => x.Value?.Errors.Count > 0)
                                         .ToDictionary(x => x.Key, x => x.Value?.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var result = await _jwtService.RegisterAsync(request);
                if (result == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Code = "REGISTRATION_FAILED",
                        Parameters = new Dictionary<string, object> { { "message", "No se pudo registrar el usuario. El email puede estar en uso." } }
                    });
                }

                _logger.LogInformation("User registered successfully: {Email}", request.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Code = "SERVER_ERROR",
                    Parameters = new Dictionary<string, object> { { "message", "Error interno del servidor" } }
                });
            }
        }

        /// <summary>
        /// Iniciar sesión
        /// </summary>
        /// <param name="request">Credenciales de login</param>
        /// <returns>Tokens de autenticación y datos del usuario</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Code = "INVALID_LOGIN_DATA",
                        Parameters = new Dictionary<string, object> { { "message", "Datos de login inválidos" } }
                    });
                }

                var result = await _jwtService.AuthenticateAsync(request.Email, request.Password);
                if (result == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Code = "INVALID_CREDENTIALS",
                        Parameters = new Dictionary<string, object> { { "message", "Credenciales inválidas" } }
                    });
                }

                _logger.LogInformation("User logged in successfully: {Email}", request.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Code = "SERVER_ERROR",
                    Parameters = new Dictionary<string, object> { { "message", "Error interno del servidor" } }
                });
            }
        }

        /// <summary>
        /// Refrescar tokens de autenticación
        /// </summary>
        /// <param name="request">Access token y refresh token</param>
        /// <returns>Nuevos tokens de autenticación</returns>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Refresh([FromBody] TokenRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.RefreshToken))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Code = "TOKENS_REQUIRED",
                        Parameters = new Dictionary<string, object> { { "message", "Access token y refresh token son requeridos" } }
                    });
                }

                var result = await _jwtService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);
                if (result == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Code = "INVALID_TOKENS",
                        Parameters = new Dictionary<string, object> { { "message", "Tokens inválidos o expirados" } }
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Code = "SERVER_ERROR",
                    Parameters = new Dictionary<string, object> { { "message", "Error interno del servidor" } }
                });
            }
        }

        /// <summary>
        /// Revocar un refresh token específico
        /// </summary>
        /// <param name="request">Refresh token a revocar</param>
        /// <returns>Confirmación de revocación</returns>
        [HttpPost("revoke")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Revoke([FromBody] TokenRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RefreshToken))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Code = "REFRESH_TOKEN_REQUIRED",
                        Parameters = new Dictionary<string, object> { { "message", "Refresh token es requerido" } }
                    });
                }

                var result = await _jwtService.RevokeTokenAsync(request.RefreshToken);
                if (!result)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Code = "INVALID_REFRESH_TOKEN",
                        Parameters = new Dictionary<string, object> { { "message", "Refresh token inválido" } }
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Code = "TOKEN_REVOKED",
                    Parameters = new Dictionary<string, object> { { "message", "Token revocado exitosamente" } }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token revocation");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Code = "SERVER_ERROR",
                    Parameters = new Dictionary<string, object> { { "message", "Error interno del servidor" } }
                });
            }
        }

        /// <summary>
        /// Revocar todos los refresh tokens del usuario actual
        /// </summary>
        /// <returns>Confirmación de revocación</returns>
        [HttpPost("revoke-all")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RevokeAll()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Code = "USER_NOT_IDENTIFIED",
                        Parameters = new Dictionary<string, object> { { "message", "Usuario no identificado" } }
                    });
                }

                await _jwtService.RevokeAllTokensAsync(userId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Code = "ALL_TOKENS_REVOKED",
                    Parameters = new Dictionary<string, object> { { "message", "Todos los tokens han sido revocados exitosamente" } }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during revoke all tokens");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Code = "SERVER_ERROR",
                    Parameters = new Dictionary<string, object> { { "message", "Error interno del servidor" } }
                });
            }
        }

        /// <summary>
        /// Obtener información del usuario autenticado
        /// </summary>
        /// <returns>Datos del usuario actual</returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserInfo>), StatusCodes.Status200OK)]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var firstName = User.FindFirst("firstName")?.Value;
                var lastName = User.FindFirst("lastName")?.Value;

                var userInfo = new UserInfo
                {
                    Id = userId ?? string.Empty,
                    Email = email ?? string.Empty,
                    FirstName = firstName ?? string.Empty,
                    LastName = lastName ?? string.Empty
                };

                return Ok(new ApiResponse<UserInfo>
                {
                    Success = true,
                    Code = "USER_INFO_RETRIEVED",
                    Data = userInfo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user info");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Code = "SERVER_ERROR",
                    Parameters = new Dictionary<string, object> { { "message", "Error interno del servidor" } }
                });
            }
        }
    }
}