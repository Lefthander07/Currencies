using Fuse8.BackendInternship.InternalApi.ApiModels;
using Microsoft.EntityFrameworkCore;

namespace Fuse8.BackendInternship.InternalApi.Data;

public class CurrencyCacheRepository
{
    private readonly CurrencyDbContext _dbContext;

    public CurrencyCacheRepository(CurrencyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<CurrencyCache?> GetLatestCacheAsync(string baseCurrency, TimeSpan expiration, CancellationToken cancellationToken)
    {
        return  _dbContext.CurrencyCaches
            .Where(c => c.BaseCurrency == baseCurrency &&
                        DateTime.UtcNow - c.CacheDate < expiration)
            .Include(c => c.ExchangeRates)
            .OrderByDescending(c => c.CacheDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<CurrencyCache?> GetCacheByDateAsync(string baseCurrency, DateOnly date, CancellationToken cancellationToken)
    {
            return _dbContext.CurrencyCaches
        .Where(c => c.BaseCurrency == baseCurrency && DateOnly.FromDateTime(c.CacheDate) == date)
        .OrderByDescending(c => c.CacheDate)
        .Include(c => c.ExchangeRates)
        .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Создает новый кэш и возвращает его в место вызова
    /// </summary>
    /// <param name="cacheBase">Базовая валюта кэш</param>
    /// <param name="cacheDate">Время, на который курс валют актуален (для исторического запроса передается дата с ответа внешнего API, для текущего берется текущее UTC</param>
    /// <param name="currencies">Курсы валют</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    public async Task<CurrencyCache?> CreateCacheAsync(string cacheBase,DateTime cacheDate, CurrencyExchangeRate[] currencies, CancellationToken cancellationToken)
    {

        var freshCache = new CurrencyCache
        {
            BaseCurrency = cacheBase,
            CacheDate = cacheDate,
            ExchangeRates = currencies.Select(currency => new CurrencyExchange
            {
                CurrencyCode = currency.CurrencyCode,
                ExchangeRate = currency.Value
            }).ToList()
        };

        _dbContext.CurrencyCaches.Add(freshCache);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return freshCache;
    }




}
