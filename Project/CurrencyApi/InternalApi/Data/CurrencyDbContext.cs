using Microsoft.EntityFrameworkCore;

namespace Fuse8.BackendInternship.InternalApi.Data;

public class CurrencyDbContext : DbContext
{
    public const string SchemaName = "cur";

    public CurrencyDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public DbSet<CurrencyCache> CurrencyCaches { get; set; }
    public DbSet<CurrencyExchange> CurrencyExchangeRates { get; set; }



}
