namespace Fuse8.BackendInternship.PublicApi.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


public class SelectedExchangeRatesConfiguration : IEntityTypeConfiguration<SelectedExchangeRate>
{
    public void Configure(EntityTypeBuilder<SelectedExchangeRate> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.Name).IsUnique();
        builder.HasIndex(p => new {p.CurrencyCode, p.BaseCurrency}).IsUnique();

        builder.Property(p => p.CurrencyCode)
            .HasMaxLength(5);

        builder.Property(p => p.BaseCurrency)
            .HasMaxLength(5);

        builder.Property(p => p.Name).HasMaxLength(100);
    }
}
