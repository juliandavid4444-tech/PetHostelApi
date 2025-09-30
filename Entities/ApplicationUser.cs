using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PetHostelApi.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]  
        public string LastName { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property para refresh tokens
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}