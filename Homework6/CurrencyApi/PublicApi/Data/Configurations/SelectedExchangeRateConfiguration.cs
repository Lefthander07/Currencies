namespace Fuse8.BackendInternship.PublicApi.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


public class SelectedExchangeRatesConfiguration : IEntityTypeConfiguration<SelectedExchangeRates>
{
    public void Configure(EntityTypeBuilder<SelectedExchangeRates> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.Name).IsUnique();
        builder.HasIndex(p => new {p.CurrencyCode, p.BaseCurrency}).IsUnique();

        builder.Property(p => p.CurrencyCode)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(p => p.BaseCurrency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(p => p.Name)
            .IsRequired();
    }
}
