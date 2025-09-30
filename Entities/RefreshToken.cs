using System.ComponentModel.DataAnnotations;

namespace PetHostelApi.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Token { get; set; } = string.Empty;
        
        [Required]
        public string JwtId { get; set; } = string.Empty;
        
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        
        public DateTime ExpiryDate { get; set; }
        
        public bool Used { get; set; } = false;
        
        public bool Invalidated { get; set; } = false;
        
        // Foreign key
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        // Navigation property
        public virtual ApplicationUser User { get; set; } = null!;
    }
}