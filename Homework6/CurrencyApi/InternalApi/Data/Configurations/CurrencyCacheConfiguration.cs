using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fuse8.BackendInternship.InternalApi.Data.Configurations;

public class CurrencyCacheConfiguration : IEntityTypeConfiguration<CurrencyCache>
{
    public void Configure(EntityTypeBuilder<CurrencyCache> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.BaseCurrency).IsRequired();
        builder.Property(p => p.CacheDate).IsRequired();
    }
}
