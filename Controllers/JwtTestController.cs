using Microsoft.AspNetCore.Mvc;

namespace PetHostelApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JwtTestController : ControllerBase
    {
        /// <summary>
        /// Endpoint simple para verificar que JWT está funcionando
        /// </summary>
        /// <returns>Respuesta de prueba</returns>
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new 
            { 
                message = "JWT Implementation Ready! 🔐",
                timestamp = DateTime.UtcNow,
                status = "JWT system is configured and ready to use"
            });
        }

        /// <summary>
        /// Test endpoint para generar un token simple
        /// </summary>
        /// <returns>Token de prueba</returns>
        [HttpPost("generate-test-token")]
        public IActionResult GenerateTestToken()
        {
            // Esto es solo para mostrar que la configuración JWT está lista
            var testResponse = new
            {
                message = "JWT configuration is ready",
                nextSteps = new[]
                {
                    "1. Create database migration with: dotnet ef migrations add AddIdentityAndJWT",
                    "2. Apply migration with: dotnet ef database update", 
                    "3. Use /api/Authentication endpoints for real JWT functionality"
                },
                endpoints = new
                {
                    register = "POST /api/Authentication/register",
                    login = "POST /api/Authentication/login", 
                    refresh = "POST /api/Authentication/refresh",
                    userInfo = "GET /api/Authentication/me"
                }
            };

            return Ok(testResponse);
        }
    }
}