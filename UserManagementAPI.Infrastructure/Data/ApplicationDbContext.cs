using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Domain.Entities;
using UserManagementAPI.Domain.Enums;

namespace UserManagementAPI.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.Position).HasMaxLength(100);
                entity.Property(e => e.Role).HasConversion<int>();
                entity.Property(e => e.Status).HasConversion<int>();

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var adminUser = new User
            {
                Id = Guid.Parse("91478E0E-C9AF-4BF0-A651-F5E91116FEDB"),
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@system.com",
                PhoneNumber = "+2347031204544",
                PasswordHash = "$2b$10$mN7OJG7Qo.ffWTr7svVsoerYBkj7THDA86Rjd6eCarNg8sa/yyKM.",
                Role = UserRole.Admin,
                Status = UserStatus.Active,
                Department = "IT",
                Position = "System Administrator",
                CreatedAt = new DateTime(2024, 07, 12),
                UpdatedAt = new DateTime(2025, 07, 12) 
            };

            modelBuilder.Entity<User>().HasData(adminUser);
        }

    }
}