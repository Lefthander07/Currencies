using Fuse8.BackendInternship.InternalApi.ApiModels;
using Fuse8.BackendInternship.InternalApi.Configurations;
using Fuse8.BackendInternship.InternalApi.Contracts;
using Fuse8.BackendInternship.InternalApi.Data;
using Fuse8.BackendInternship.Exceptions;
using Microsoft.Extensions.Options;

public class CashedCurrency_DB : ICachedCurrencyAPI
{
    private readonly CurrencyHttpApi _currencyAPI;
    private readonly CurrencyCacheRepository _currencyCacheRepository;
    private readonly TimeSpan _cacheExpiration;
    private const string CACHE_BASE  = "USD";

    public CashedCurrency_DB(CurrencyHttpApi currencyAPI, IOptionsSnapshot<CurrencyOptions> apiSettings, CurrencyCacheRepository currencyCacheRepository)
    {
        _currencyAPI = currencyAPI;
        _cacheExpiration = TimeSpan.FromHours(apiSettings.Value.cacheExpiration);
        _currencyCacheRepository = currencyCacheRepository;
    }

    /// <summary>
    /// Получает актуальный курс указанной валюты по отношению к базовой валюте.
    /// Если валюта совпадает с базовой, возвращается значение 1.
    /// В случае отсутствия актуального кэша, он запрашивается у внешнего API и сохраняется.
    /// </summary>
    /// <param name="currencyType">Код валюты, для которой требуется получить курс.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Объект <see cref="CurrencyExchangeRate"/> с кодом валюты и её курсом по отношению к базовой валюте.</returns>
    /// <exception cref="CurrencyNotFoundException">Выбрасывается, если указанный код валюты не найден в кэше.</exception>
    public async Task<CurrencyExchangeRate> GetCurrentCurrencyAsync(string currencyType, CancellationToken cancellationToken = default)
    {   
        if (currencyType == CACHE_BASE )
        {
            return new CurrencyExchangeRate { CurrencyCode = currencyType, Value = 1 };
        }

        var freshCache = await _currencyCacheRepository.GetLatestCacheAsync(CACHE_BASE, _cacheExpiration, cancellationToken);

        CurrencyExchange? exchangeRate;

        if (freshCache == null)
        {
            var currencies = await _currencyAPI.GetAllCurrentCurrenciesAsync(CACHE_BASE, cancellationToken);
            freshCache = await _currencyCacheRepository.CreateCacheAsync(CACHE_BASE, DateTime.UtcNow, currencies, cancellationToken);
        }

        exchangeRate = freshCache!.ExchangeRates!.FirstOrDefault(er => er.CurrencyCode == currencyType);


        if (exchangeRate == null)
        {
            throw new CurrencyNotFoundException($"Валюта {currencyType} не найдена.");
        }

        return new CurrencyExchangeRate
        {
            CurrencyCode = exchangeRate!.CurrencyCode,
            Value = exchangeRate.ExchangeRate
        };
    }

    /// <summary>
    /// Получает курс указанной валюты по отношению к базовой валюте на заданную дату.
    /// Если валюта совпадает с базовой, возвращается значение 1.
    /// В случае отсутствия кэша за указанную дату, данные загружаются с внешнего API и сохраняются.
    /// </summary>
    /// <param name="currencyType">Код валюты, для которой требуется получить курс.</param>
    /// <param name="date">Дата, на которую необходимо получить курс валюты.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Объект <see cref="CurrencyExchangeRate"/>, содержащий код валюты и её курс на указанную дату.</returns>
    /// <exception cref="CurrencyNotFoundException">Выбрасывается, если указанный код валюты не найден в кэше за выбранную дату.</exception>
    public async Task<CurrencyExchangeRate> GetCurrencyOnDateAsync(string currencyType, DateOnly date, CancellationToken cancellationToken = default)
    {
        if (currencyType == CACHE_BASE )
        {
            return new CurrencyExchangeRate { CurrencyCode = currencyType, Value = 1 };
        }

        var freshCache = await _currencyCacheRepository.GetCacheByDateAsync(CACHE_BASE, date, cancellationToken);

        CurrencyExchange? exchangeRate;

        if (freshCache == null)
        {
            var currencies = await _currencyAPI.GetAllCurrenciesOnDateAsync(CACHE_BASE, date, cancellationToken);
            freshCache = await _currencyCacheRepository.CreateCacheAsync(CACHE_BASE, currencies.LastUpdatedAt, currencies.Currencies, cancellationToken);
        }

        exchangeRate = freshCache!.ExchangeRates.FirstOrDefault(er => er.CurrencyCode == currencyType);


        if (exchangeRate == null)
        {
            throw new CurrencyNotFoundException($"Валюта {currencyType} не найдена.");
        }

        return new CurrencyExchangeRate
        {
            CurrencyCode = exchangeRate!.CurrencyCode,
            Value = exchangeRate.ExchangeRate
        };
    }
}

