using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Models;

namespace WarehouseAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Roles).HasMaxLength(500);
        });
        
        // Seed data for testing
        SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Create a test admin user
        // Password: "admin123"
        var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
        
        // Create a test regular user
        // Password: "user123"
        var userPasswordHash = BCrypt.Net.BCrypt.HashPassword("user123");
        
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Email = "admin@warehouse.com",
                PasswordHash = adminPasswordHash,
                Roles = "Admin,User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 2,
                Email = "user@warehouse.com",
                PasswordHash = userPasswordHash,
                Roles = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}