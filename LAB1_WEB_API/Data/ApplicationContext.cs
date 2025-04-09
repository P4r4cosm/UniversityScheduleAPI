using LAB1_WEB_API.Services;
using Microsoft.EntityFrameworkCore;

namespace LAB1_WEB_API;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.EnsureCreated(); // создаем базу данных при первом обращении
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Настройка уникального индекса для Username
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Name)
            .IsUnique();
       
    }
}