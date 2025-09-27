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
        private readonly AuthService? _authService;

        public AuthController(ILogger<AuthController> logger, AuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("login")]
        public ActionResult<User> Login(LoginRequest request)
        {
            var user = _authService?.Authenticate(request.user, request.password);
            if (user == null)
            {
                return Unauthorized(); 
            }
            return Ok(user);
        }
    }
}

