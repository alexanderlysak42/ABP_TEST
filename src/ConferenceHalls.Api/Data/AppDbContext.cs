using ConferenceHalls.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceHalls.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {}
    
    public DbSet<Hall> Halls => Set<Hall>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hall>(entity =>
        {
            entity.Property(h => h.Name).IsRequired().HasMaxLength(100);
            entity.Property(h => h.BaseHourlyPrice).HasPrecision(18, 2);
            entity.HasIndex(h => h.Name).IsUnique();
        });

        modelBuilder.Entity<Hall>().HasData(
            new Hall { Id = 1, Name = "Зал A", Capacity = 50, BaseHourlyPrice = 2000m },
            new Hall { Id = 2, Name = "Зал B", Capacity = 100, BaseHourlyPrice = 3500m },
            new Hall { Id = 3, Name = "Зал C", Capacity = 30, BaseHourlyPrice = 1500m });
    }
}