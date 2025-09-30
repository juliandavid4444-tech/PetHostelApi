using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetHostelApi.Contexts;
using PetHostelApi.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace PetHostelApi.Services
{
    public interface IJwtService
    {
        Task<AuthenticationResponse?> AuthenticateAsync(string email, string password);
        Task<AuthenticationResponse?> RegisterAsync(RegisterRequest request);
        Task<AuthenticationResponse?> RefreshTokenAsync(string accessToken, string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken);
        Task<bool> RevokeAllTokensAsync(string userId);
    }

    public class JwtService : IJwtService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<JwtService> _logger;

        public JwtService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDbContext context,
            IOptions<JwtSettings> jwtSettings,
            ILogger<JwtService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<AuthenticationResponse?> AuthenticateAsync(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("Authentication failed: User not found for email {Email}", email);
                    return null;
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Authentication failed: Invalid credentials for user {UserId}", user.Id);
                    return null;
                }

                return await GenerateAuthenticationResponseAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for email {Email}", email);
                return null;
            }
        }

        public async Task<AuthenticationResponse?> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Registration failed: User already exists for email {Email}", request.Email);
                    return null;
                }

                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Registration failed for email {Email}: {Errors}", 
                        request.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return null;
                }

                _logger.LogInformation("User registered successfully: {UserId}", user.Id);
                return await GenerateAuthenticationResponseAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email {Email}", request.Email);
                return null;
            }
        }

        public async Task<AuthenticationResponse?> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(accessToken);
                if (principal == null)
                {
                    _logger.LogWarning("Refresh token failed: Invalid access token");
                    return null;
                }

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Refresh token failed: User ID not found in token");
                    return null;
                }

                var storedRefreshToken = await _context.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

                if (storedRefreshToken == null || 
                    storedRefreshToken.Used || 
                    storedRefreshToken.Invalidated || 
                    storedRefreshToken.ExpiryDate <= DateTime.UtcNow)
                {
                    _logger.LogWarning("Refresh token failed: Invalid refresh token for user {UserId}", userId);
                    return null;
                }

                // Mark the refresh token as used
                storedRefreshToken.Used = true;
                await _context.SaveChangesAsync();

                return await GenerateAuthenticationResponseAsync(storedRefreshToken.User);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during refresh token operation");
                return null;
            }
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            try
            {
                var token = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

                if (token == null) return false;

                token.Invalidated = true;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Refresh token revoked for user {UserId}", token.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking refresh token");
                return false;
            }
        }

        public async Task<bool> RevokeAllTokensAsync(string userId)
        {
            try
            {
                var tokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId && !rt.Invalidated)
                    .ToListAsync();

                foreach (var token in tokens)
                {
                    token.Invalidated = true;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("All refresh tokens revoked for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking all tokens for user {UserId}", userId);
                return false;
            }
        }

        private async Task<AuthenticationResponse> GenerateAuthenticationResponseAsync(ApplicationUser user)
        {
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

            var accessToken = GenerateAccessToken(user, accessTokenExpiration);
            var refreshToken = GenerateRefreshToken();

            // Store refresh token in database
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                JwtId = GetJwtId(accessToken),
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = refreshTokenExpiration,
                Used = false,
                Invalidated = false
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthenticationResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration,
                User = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                }
            };
        }

        private string GenerateAccessToken(ApplicationUser user, DateTime expiration)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private string GetJwtId(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateLifetime = false // We don't care about expiration for refresh
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                
                if (validatedToken is not JwtSecurityToken jwtSecurityToken || 
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}