using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fuse8.BackendInternship.InternalApi.Data.Configurations
{
    public class CurrencyExchangeRateConfiguration : IEntityTypeConfiguration<CurrencyExchange>
    {
        public void Configure(EntityTypeBuilder<CurrencyExchange> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.CurrencyCode).IsRequired();
            builder.Property(p => p.ExchangeRate).IsRequired();

            builder
                .HasOne(p => p.CurrencyCache)
                .WithMany(p => p.ExchangeRates)
                .HasForeignKey(p => p.CurrencyCacheId);
                
        }

    }
}
