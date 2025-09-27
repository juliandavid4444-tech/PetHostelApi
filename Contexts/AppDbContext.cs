using PetHostelApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace PetHostelApi.Contexts
{
	public class AppDbContext: DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
		{
		}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseSqlServer("Server=tcp:pethostel.database.windows.net,1433;Initial Catalog=PetHostelDB;Persist Security Info=False;User ID=juliandavid333;Password=JUlianb@501234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }

        public DbSet<Commerce> Commerce { get; set; }

        public DbSet<User> User { get; set; }
    }
}

