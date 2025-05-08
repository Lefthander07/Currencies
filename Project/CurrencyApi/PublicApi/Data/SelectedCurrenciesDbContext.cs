using Microsoft.EntityFrameworkCore;

namespace Fuse8.BackendInternship.PublicApi.Data;

public class SelectedCurrenciesDbContext : DbContext
{
    public const string SchemaName = "user";
    public SelectedCurrenciesDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public DbSet<SelectedExchangeRate> SelectedExchangeRates { get; set; }
}
