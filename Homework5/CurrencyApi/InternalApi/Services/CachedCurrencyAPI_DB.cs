using Fuse8.BackendInternship.InternalApi.ApiModels;
using Fuse8.BackendInternship.InternalApi.Configurations;
using Fuse8.BackendInternship.InternalApi.Contracts;
using Fuse8.BackendInternship.InternalApi.Data;
using Fuse8.BackendInternship.InternalApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


public class CashedCurrency_DB : ICachedCurrencyAPI
{
    private readonly CurrencyHttpApi _currencyAPI;
    private readonly CurrencyDbContext _dbContext;
    private readonly TimeSpan _cacheExpiration;
    private readonly string _cachedDirectory;
    private const string CACHE_BASE  = "USD";


    public CashedCurrency_DB(CurrencyHttpApi currencyAPI, IOptionsSnapshot<CurrencyOptions> apiSettings, CurrencyDbContext dbContext)
    {

        _currencyAPI = currencyAPI;
        _cacheExpiration = TimeSpan.FromHours(apiSettings.Value.cacheExpiration);
        _cachedDirectory = "CurrencyCache";
        Directory.CreateDirectory(_cachedDirectory);
        _dbContext = dbContext;
    }

    public async Task<CurrencyExchangeRate> GetCurrentCurrencyAsync(string currencyType, CancellationToken cancellationToken = default)
    {
        
        if (currencyType == CACHE_BASE )
        {
            return new CurrencyExchangeRate { CurrencyCode = currencyType, Value = 1 };
        }

        var freshCache = await _dbContext.CurrencyCaches
            .Where(c => c.BaseCurrency == CACHE_BASE &&
                        DateTime.UtcNow - c.CacheDate < _cacheExpiration)
            .Include(c => c.ExchangeRates)
            .FirstOrDefaultAsync(cancellationToken);

        CurrencyExchange exchangeRate;

        if (freshCache == null)
        {
            var currencies = await _currencyAPI.GetAllCurrentCurrenciesAsync(CACHE_BASE, cancellationToken);
            var cacheMeta = new CurrencyCache
            {
                BaseCurrency = CACHE_BASE,
                CacheDate = DateTime.UtcNow
            };

            _dbContext.CurrencyCaches.Add(cacheMeta);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var currencyExchangeRates = currencies.Select(currency => new CurrencyExchange
            {
                CurrencyCacheId = cacheMeta.Id,
                CurrencyCode = currency.CurrencyCode,
                ExchangeRate = currency.Value
            }).ToList();

            _dbContext.CurrencyExchangeRates.AddRange(currencyExchangeRates);
            await _dbContext.SaveChangesAsync(cancellationToken);

            exchangeRate = currencyExchangeRates.FirstOrDefault(er => er.CurrencyCode == currencyType);
        }
        else
        {
            exchangeRate = freshCache.ExchangeRates.FirstOrDefault(er => er.CurrencyCode == currencyType);
        }

        if (exchangeRate == null)
        {
            throw new CurrencyNotFoundException($"Валюта {currencyType} не найдена.");
        }

        return new CurrencyExchangeRate
        {
            CurrencyCode = exchangeRate.CurrencyCode,
            Value = exchangeRate.ExchangeRate
        };
    }

    public async Task<CurrencyExchangeRate> GetCurrencyOnDateAsync(string currencyType, DateOnly date, CancellationToken cancellationToken = default)
    {
        if (currencyType == CACHE_BASE )
        {
            return new CurrencyExchangeRate { CurrencyCode = currencyType, Value = 1 };
        }

        var freshCache = await _dbContext.CurrencyCaches
            .Where(c => c.BaseCurrency == CACHE_BASE && DateOnly.FromDateTime(c.CacheDate) == date)
            .OrderByDescending(c => c.CacheDate)
            .Include(c => c.ExchangeRates)
            .FirstOrDefaultAsync(cancellationToken);

        CurrencyExchange exchangeRate;

        if (freshCache == null)
        {
            var currencies = await _currencyAPI.GetAllCurrenciesOnDateAsync(CACHE_BASE, date, cancellationToken);
            var cacheMeta = new CurrencyCache
            {
                BaseCurrency = CACHE_BASE,
                CacheDate = currencies.LastUpdatedAt
            };

            _dbContext.CurrencyCaches.Add(cacheMeta);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var currencyExchangeRates = currencies.Currencies.Select(currency => new CurrencyExchange
            {
                CurrencyCacheId = cacheMeta.Id,
                CurrencyCode = currency.CurrencyCode,
                ExchangeRate = currency.Value
            }).ToList();

            _dbContext.CurrencyExchangeRates.AddRange(currencyExchangeRates);
            await _dbContext.SaveChangesAsync(cancellationToken);

            exchangeRate = currencyExchangeRates.FirstOrDefault(er => er.CurrencyCode == currencyType);
        }
        else
        {
            exchangeRate = freshCache.ExchangeRates.FirstOrDefault(er => er.CurrencyCode == currencyType);
        }

        if (exchangeRate == null)
        {
            throw new CurrencyNotFoundException($"Валюта {currencyType} не найдена.");
        }

        return new CurrencyExchangeRate
        {
            CurrencyCode = exchangeRate.CurrencyCode,
            Value = exchangeRate.ExchangeRate
        };
    }
}
