using Fuse8.BackendInternship.InternalApi.ApiModels;
using Fuse8.BackendInternship.InternalApi.Configurations;
using Fuse8.BackendInternship.InternalApi.Contracts;
using Fuse8.BackendInternship.InternalApi.Exceptions;
using Microsoft.Extensions.Options;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class CashedCurrency : ICachedCurrencyAPI
{
    private readonly CurrencyHttpApi _currencyAPI;
    private readonly TimeSpan _cacheExpiration;
    private readonly string _cachedDirectory;
    private readonly string _baseCurrency;

    public CashedCurrency(CurrencyHttpApi currencyAPI, IOptionsSnapshot<CurrencyOptions> apiSettings)
    {
        _currencyAPI = currencyAPI;
        _cacheExpiration = TimeSpan.FromHours(apiSettings.Value.cacheExpiration);
        _cachedDirectory = "CurrencyCache";
        Directory.CreateDirectory(_cachedDirectory);
       _baseCurrency = apiSettings.Value.BaseCurrency;
    }

    public async Task<CurrencyExchangeRate> GetCurrentCurrencyAsync(string currencyType, CancellationToken cancellationToken  = default)
    {
        var lastFileOnDate = GetCurrencyCacheFiles(_baseCurrency)
            .Select(filePath => new
            {
                FilePath = filePath,
                DateTime = ExtractDateTimeFromFileName(filePath),
            })
            .Where(file => DateOnly.FromDateTime(file.DateTime) == DateOnly.FromDateTime(DateTime.Now))
            .MaxBy(file => file.DateTime);

            if (lastFileOnDate is not null && (DateTime.Now - lastFileOnDate.DateTime < _cacheExpiration) )
            {
                return await GetCurrencyFromCacheAsync(lastFileOnDate.FilePath, currencyType, cancellationToken);
            }
        return await FetchAndCacheCurrentCurrencyAsync(currencyType, cancellationToken);

    }

    public async Task<CurrencyExchangeRate> GetCurrencyOnDateAsync(string currencyType, DateOnly date, CancellationToken cancellationToken = default)
    {
        var lastFileOnDate = GetCurrencyCacheFiles(_baseCurrency)
            .Select(filePath => new
            {
                FilePath = filePath,
                DateTime = ExtractDateTimeFromFileName(filePath),
            })
            .Where(file => DateOnly.FromDateTime(file.DateTime) == date)
            .MaxBy(file => file.DateTime);

        if (lastFileOnDate is null)
        {
            return await FetchAndCacheCurrencyOnDateAsync(currencyType, date, cancellationToken);
        }

        return await GetCurrencyFromCacheAsync(lastFileOnDate.FilePath, currencyType, cancellationToken);
    }

    private DateTime ExtractDateTimeFromFileName(string filePath)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var parts = fileName.Split('_');
        return DateTime.ParseExact(parts[2], "yyyyMMddTHHmmss", null);
    }

    private string GetCurrencyFilePrefix(string currencyType)
    {
        return $"currency_{currencyType}";
    }

    private IEnumerable<string> GetCurrencyCacheFiles(string currencyType)
    {
        var currencyPrefix = GetCurrencyFilePrefix(currencyType);
        return Directory.EnumerateFiles(_cachedDirectory, $"{currencyPrefix}_*.json");

    }

    private string GenerateCacheFileName(string currencyType, DateTime dateTime)
    {
        var prefix = GetCurrencyFilePrefix(currencyType);
        return Path.Combine(_cachedDirectory, $"{prefix}_{dateTime:yyyyMMddTHHmmss}.json");
    }

    private async Task SaveToCacheAsync(string fileName, object data, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(data);
        await File.WriteAllTextAsync(fileName, json, cancellationToken);
    }

    private CurrencyExchangeRate FindCurrency(CurrencyExchangeRate[] currencies, string currencyType)
    {
        var currency = currencies.FirstOrDefault(c => c.CurrencyCode.Equals(currencyType, StringComparison.OrdinalIgnoreCase));
        if (currency == null)
            throw new CurrencyNotFoundException($"Валюта {currencyType} не найдена.");

        return new CurrencyExchangeRate
        {
            CurrencyCode = currency.CurrencyCode,
            Value = currency.Value,
        };
    }

    private async Task<CurrencyExchangeRate> FetchAndCacheCurrentCurrencyAsync(string currencyType, CancellationToken cancellationToken)
    {
        var currencies = await _currencyAPI.GetAllCurrentCurrenciesAsync(_baseCurrency, cancellationToken);
        var fileName = GenerateCacheFileName(_baseCurrency, DateTime.Now);
        await SaveToCacheAsync(fileName, currencies, cancellationToken);
        return FindCurrency(currencies, currencyType);
    }

    private async Task<CurrencyExchangeRate> FetchAndCacheCurrencyOnDateAsync(string currencyType, DateOnly date, CancellationToken cancellationToken)
    {
        var currenciesOnDate = await _currencyAPI.GetAllCurrenciesOnDateAsync(
            _baseCurrency,
            date,
            cancellationToken);

        var fileName = GenerateCacheFileName(_baseCurrency, currenciesOnDate.LastUpdatedAt.ToLocalTime());
        await SaveToCacheAsync(fileName, currenciesOnDate.Currencies, cancellationToken);

        return FindCurrency(currenciesOnDate.Currencies, currencyType);
    }

    private async Task<CurrencyExchangeRate> GetCurrencyFromCacheAsync(string fileName, string currencyType, CancellationToken cancellationToken)
    {
        var cacheData = await LoadFromCacheAsync(fileName, cancellationToken);
        return FindCurrency(cacheData, currencyType);
    }
    private async Task<CurrencyExchangeRate[]> LoadFromCacheAsync(string fileName, CancellationToken cancellationToken)
    {
        var json = await File.ReadAllTextAsync(fileName);
        return JsonSerializer.Deserialize<CurrencyExchangeRate[]>(json);
    }
}
