using Microsoft.EntityFrameworkCore;

namespace Fuse8.BackendInternship.InternalApi.Data;

public class CurrencyDbContext : DbContext
{
    public CurrencyDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /// Устанавливаю схемуу по умолчанию
        modelBuilder.HasDefaultSchema("cur");

        /// автоматическое применение всех конфигураций в текущей сборке
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public DbSet<CurrencyCache> CurrencyCaches { get; set; }
    public DbSet<CurrencyExchange> CurrencyExchangeRates { get; set; }



}
