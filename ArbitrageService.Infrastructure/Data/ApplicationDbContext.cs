using ArbitrageService.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ArbitrageService.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<FuturesPrice> FuturesPrices { get; set; }
    public DbSet<PriceDifference> PriceDifferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FuturesPrice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Symbol).IsRequired();
            entity.Property(e => e.Price).HasPrecision(18, 8);
        });

        modelBuilder.Entity<PriceDifference>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstSymbol).IsRequired();
            entity.Property(e => e.SecondSymbol).IsRequired();
            entity.Property(e => e.FirstPrice).HasPrecision(18, 8);
            entity.Property(e => e.SecondPrice).HasPrecision(18, 8);
            entity.Property(e => e.Difference).HasPrecision(18, 8);
        });
    }
}
