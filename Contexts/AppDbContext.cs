using PetHostelApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace PetHostelApi.Contexts
{
	public class AppDbContext: DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
		{
		}

        // Removido OnConfiguring para evitar conflictos con la configuración en Program.cs
        // La cadena de conexión se configura ahora en Program.cs usando appsettings.json

        public DbSet<Commerce> Commerce { get; set; }

        public DbSet<User> User { get; set; }
    }
}

