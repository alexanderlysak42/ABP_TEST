using ConferenceHalls.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceHalls.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {}
    
    public DbSet<Hall> Halls => Set<Hall>();
    
    public DbSet<AdditionalService> AdditionalServices => Set<AdditionalService>();
    
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hall>(entity =>
        {
            entity.Property(h => h.Name).IsRequired().HasMaxLength(100);
            entity.Property(h => h.BaseHourlyPrice).HasPrecision(18, 2);
            entity.HasIndex(h => h.Name).IsUnique();
        });

        modelBuilder.Entity<AdditionalService>(entity =>
        {
            entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
            entity.Property(s => s.Price).HasPrecision(18, 2);
            entity.HasOne(s => s.Hall)
                .WithMany(h => h.AdditionalServices)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(s => new { s.HallId, s.Name }).IsUnique();
        });
        
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(b => b.StartTime).HasColumnType("timestamp without time zone");
            entity.Property(b => b.EndTime).HasColumnType("timestamp without time zone");
            entity.Property(b => b.HallCost).HasPrecision(18, 2);
            entity.Property(b => b.ServicesCost).HasPrecision(18, 2);
            entity.Property(b => b.TotalCost).HasPrecision(18, 2);
            entity.HasOne(b => b.Hall)
                .WithMany(h => h.Bookings)
                .HasForeignKey(b => b.HallId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(b => b.AdditionalServices)
                .WithMany();
            entity.HasIndex(b => new { b.HallId, b.StartTime, b.EndTime });
        });



        modelBuilder.Entity<Hall>().HasData(
            new Hall { Id = 1, Name = "Зал A", Capacity = 50, BaseHourlyPrice = 2000m },
            new Hall { Id = 2, Name = "Зал B", Capacity = 100, BaseHourlyPrice = 3500m },
            new Hall { Id = 3, Name = "Зал C", Capacity = 30, BaseHourlyPrice = 1500m });
    }
}