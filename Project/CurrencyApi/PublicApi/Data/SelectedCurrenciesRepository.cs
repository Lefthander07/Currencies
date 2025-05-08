using Microsoft.EntityFrameworkCore;

namespace Fuse8.BackendInternship.PublicApi.Data;

public class SelectedCurrenciesRepository
{
    private readonly SelectedCurrenciesDbContext _context;

    public SelectedCurrenciesRepository(SelectedCurrenciesDbContext dbContext)
    {
        _context = dbContext;
    }

    public Task<List<SelectedExchangeRate>> GetAllAsync(CancellationToken cancellationToken)
        => _context.SelectedExchangeRates.ToListAsync(cancellationToken);

    public Task<SelectedExchangeRate?> GetByNameAsync(string name, CancellationToken cancellationToken)
        => _context.SelectedExchangeRates.FirstOrDefaultAsync(e => e.Name == name, cancellationToken);

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
        => _context.SelectedExchangeRates.AnyAsync(e => e.Name == name, cancellationToken);

    public Task<bool> ExistsByCurrenciesAsync(string sourceCurrency, string baseCurrency, string? excludingName, CancellationToken cancellationToken)
        => _context.SelectedExchangeRates.AnyAsync(
            x => x.CurrencyCode == sourceCurrency && x.BaseCurrency == baseCurrency && (excludingName == null || x.Name != excludingName),
            cancellationToken);

    public void Add(SelectedExchangeRate entity)
        => _context.SelectedExchangeRates.Add(entity);

    public void Remove(SelectedExchangeRate entity)
        => _context.SelectedExchangeRates.Remove(entity);

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => _context.SaveChangesAsync(cancellationToken);
}
