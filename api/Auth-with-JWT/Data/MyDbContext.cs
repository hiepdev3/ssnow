using Auth_with_JWT.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth_with_JWT.Data
{
	public class MyDbContext : DbContext
	{
		public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
		
        public DbSet<Mark> Marks { get; set; } // Add Mark table


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Mark>().ToTable("Marks");

        }
    }
}


