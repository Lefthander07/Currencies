using Fuse8.BackendInternship.PublicApi.Data;
using Microsoft.EntityFrameworkCore;

namespace Fuse8.BackendInternship.PublicApi.Services;

public class SelectedExchangeRatesService
{
    private readonly FavoritesCurrenciesDbContext _context;

    public SelectedExchangeRatesService(FavoritesCurrenciesDbContext context)
    {
        _context = context;
    }

    public async Task<List<SelectedExchangeRates>> GetAll(CancellationToken cancellationToken)
    {
       return await _context.SelectedExchangeRates.ToListAsync(cancellationToken);
    }

    public async Task<SelectedExchangeRates> GetByNameAsync(string name, CancellationToken cancellationToken)
    {

        var existingByName = await _context.SelectedExchangeRates.FirstOrDefaultAsync(e => e.Name == name, cancellationToken);
        if (existingByName == null)
        {
            throw new ArgumentException($"Избранный курс с именем '{name}'отсутствует в базе данных.");
        }
        return existingByName;
    }

    public async Task<SelectedExchangeRates> CreateAsync(string defaultCurrency,
                                                         string baseCurrency,
                                                         string name,
                                                         CancellationToken cancellationToken)
    {
        
        var existingByName = await _context.SelectedExchangeRates.FirstOrDefaultAsync(e => e.Name == name, cancellationToken);

        if (existingByName != null)
        {
            throw new ArgumentException($"Избранный курс с именем '{name}' уже существует в базе данных.");
        }

        var existingByCurrencies = await _context.SelectedExchangeRates
            .AnyAsync(x => x.CurrencyCode == defaultCurrency && x.BaseCurrency == baseCurrency);
        
        if (existingByCurrencies)
        {
            throw new ArgumentException($"Избранный курс с {defaultCurrency} и {baseCurrency} существует.");
        }

        var newSelected = new SelectedExchangeRates
        {
            Name = name,
            CurrencyCode = defaultCurrency,
            BaseCurrency = baseCurrency
        };

        _context.SelectedExchangeRates.Add(newSelected);
        await _context.SaveChangesAsync();
        return newSelected;
    }

    public async Task DeleteAsync(string name, CancellationToken cancellationToken)
    {

        var existingByName = await _context.SelectedExchangeRates.FirstOrDefaultAsync(e => e.Name == name, cancellationToken);

        if (existingByName is null)
        {
            throw new ArgumentException($"Избранный курс с именем '{name}' отсутствует в базе.");
        }

        _context.SelectedExchangeRates.Remove(existingByName);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSelectedAsync(string name, string baseCurrency, string defaultCurrency, string newName, CancellationToken cancellationToken)
    {
        var existingByName = await _context.SelectedExchangeRates.FirstOrDefaultAsync(e => e.Name == name, cancellationToken);

        if (existingByName is null)
        {
            throw new ArgumentException($"Избранный курс с именем '{name}' отсутствует в базе.");
        }

        var existingByNewName = await _context.SelectedExchangeRates.FirstOrDefaultAsync(e => e.Name == newName, cancellationToken);

        if (existingByNewName is not null)
        {
            throw new ArgumentException($"Избранный курс с новым именем '{name}' уже присутствует в базе.");
        }

        var existingByCurrencies = await _context.SelectedExchangeRates
            .AnyAsync(x => x.CurrencyCode == defaultCurrency && x.BaseCurrency == baseCurrency);

        if (existingByCurrencies)
        {
            throw new ArgumentException($"Избранный курс с {defaultCurrency} и {baseCurrency} существует.");
        }

        existingByName.CurrencyCode = defaultCurrency;
        existingByName.BaseCurrency = baseCurrency;
        existingByName.Name = newName;
        await _context.SaveChangesAsync();
    }
}