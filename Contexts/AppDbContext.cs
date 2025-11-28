using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetHostelApi.Entities;

namespace PetHostelApi.Contexts
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Entities
        public DbSet<Commerce> Commerce { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para RefreshToken
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);
                entity.Property(rt => rt.Token).IsRequired().HasMaxLength(500);
                entity.Property(rt => rt.JwtId).IsRequired().HasMaxLength(200);
                entity.Property(rt => rt.UserId).IsRequired();
                
                // Relación con ApplicationUser
                entity.HasOne(rt => rt.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                // Índices para performance
                entity.HasIndex(rt => rt.Token).IsUnique();
                entity.HasIndex(rt => rt.JwtId);
                entity.HasIndex(rt => rt.UserId);
            });

            // Configuración para ApplicationUser
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Commerce configuration
            modelBuilder.Entity<Commerce>(entity =>
            {
                entity.HasKey(c => c.com_id);
                entity.Property(c => c.com_name).IsRequired().HasMaxLength(200);
                entity.Property(c => c.com_address).IsRequired().HasMaxLength(500);
                entity.Property(c => c.com_services).IsRequired();
            });
        }
    }
}

