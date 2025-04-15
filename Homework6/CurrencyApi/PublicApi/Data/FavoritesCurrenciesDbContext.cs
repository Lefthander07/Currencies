using Microsoft.EntityFrameworkCore;

namespace Fuse8.BackendInternship.PublicApi.Data;

public class FavoritesCurrenciesDbContext : DbContext
{
    public FavoritesCurrenciesDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("user");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public DbSet<SelectedExchangeRates> SelectedExchangeRates { get; set; }
}
