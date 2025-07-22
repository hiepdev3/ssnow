using Auth_with_JWT.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth_with_JWT.Data
{
	public class MyDbContext : DbContext
	{
		public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
		public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<MembershipTier> MembershipTiers { get; set; } // Add MembershipTier table
        public DbSet<Mark> Marks { get; set; } // Add Mark table


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Role 1-nhiều User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Field nhiều-1 User
            modelBuilder.Entity<Field>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cart nhiều-1 Field, nhiều-1 User
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Field)
                .WithMany()
                .HasForeignKey(c => c.FieldId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Voucher nhiều-1 User, nhiều-1 Field
            modelBuilder.Entity<Voucher>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Voucher>()
                .HasOne(v => v.Field)
                .WithMany()
                .HasForeignKey(v => v.FieldId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment nhiều-1 Voucher, nhiều-1 Field
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Voucher)
                .WithMany()
                .HasForeignKey(p => p.VoucherCode)
                .HasPrincipalKey(v => v.Code)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
                

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Field)
                .WithMany()
                .HasForeignKey(p => p.FieldId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Đảm bảo Code của Voucher là unique
            modelBuilder.Entity<Voucher>()
                .HasIndex(v => v.Code)
                .IsUnique();

            // Đảm bảo Email là unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.Tier)
                .WithMany(t => t.Users) // Navigation property in MembershipTier
                .HasForeignKey(u => u.TierId) // Explicitly map to TierId
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

        }
    }
}


