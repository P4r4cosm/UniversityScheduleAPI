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
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "admin" }
        );
    }
}