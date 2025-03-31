using Fuse8.BackendInternship.InternalApi.ApiModels;
using Fuse8.BackendInternship.InternalApi.Configurations;
using Fuse8.BackendInternship.InternalApi.Contracts;
using Microsoft.Extensions.Options;
using System.Text.Json;

public class CashedCurrency : ICachedCurrencyAPI
{
    private readonly CurrencyHttpApi _currencyAPI;
    private readonly TimeSpan _cacheExpirtation;
    private readonly string _cachedDirectory;
    private readonly string _baseCurrency;

    public CashedCurrency(CurrencyHttpApi currencyAPI, IOptionsSnapshot<CurrencySettigns> apiSettings)
    {
        _currencyAPI = currencyAPI;
        _cacheExpirtation = TimeSpan.FromHours(2);
        _cachedDirectory = "CurrencyCache";
        Directory.CreateDirectory(_cachedDirectory);
       _baseCurrency = apiSettings.Value.BaseCurrency;
    }

    private DateTime ExtractDateTimeFromFileName(string filePath)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var parts = fileName.Split('_');
        return DateTime.ParseExact(parts[2], "yyyyMMddTHHmmss", null);
    }

    private DateOnly ExtractDateFromFileName(string filePath)
    {
        var dt = ExtractDateTimeFromFileName(filePath);
        return DateOnly.FromDateTime(dt);
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

    private async Task SaveToCache(string fileName, object data)
    {
        var json = JsonSerializer.Serialize(data);
        await File.WriteAllTextAsync(fileName, json);
    }

    private CurrencyExchangeRate FindCurrency(CurrencyExchangeRate[] currencies, string currencyType)
    {
        var currency = currencies.FirstOrDefault(c => c.CurrencyCode.Equals(currencyType, StringComparison.OrdinalIgnoreCase));
        if (currency == null)
            throw new KeyNotFoundException($"Currency {currencyType} not found");

        return new CurrencyExchangeRate
        {
            CurrencyCode = currency.CurrencyCode,
            Value = currency.Value,
        };
    }

    private async Task<CurrencyExchangeRate> FetchAndCacheCurrentCurrency(string currencyType, CancellationToken cancellationToken)
    {
        var currencies = await _currencyAPI.GetAllCurrentCurrenciesAsync(_baseCurrency, cancellationToken);
        var fileName = GenerateCacheFileName(_baseCurrency, DateTime.Now);
        await SaveToCache(fileName, currencies);

        return FindCurrency(currencies, currencyType);
    }

    private async Task<CurrencyExchangeRate> FetchAndCacheCurrencyOnDate(string currencyType, DateOnly date, CancellationToken cancellationToken)
    {
        var currenciesOnDate = await _currencyAPI.GetAllCurrenciesOnDateAsync(
            _baseCurrency,
            date,
            cancellationToken);

        var fileName = GenerateCacheFileName(_baseCurrency, currenciesOnDate.LastUpdatedAt);
        await SaveToCache(fileName, currenciesOnDate.Currencies.ToArray());

        return FindCurrency(currenciesOnDate.Currencies, currencyType);
    }

    private async Task<CurrencyExchangeRate> GetCurrencyFromCache(string fileName, string currencyType)
    {
        var cacheData = await LoadFromCache(fileName);
        return FindCurrency(cacheData, currencyType);
    }
    private async Task<CurrencyExchangeRate[]> LoadFromCache(string fileName)
    {
        var json = await File.ReadAllTextAsync(fileName);
        return JsonSerializer.Deserialize<CurrencyExchangeRate[]>(json);
    }

    public async Task<CurrencyExchangeRate> GetCurrentCurrencyAsync(string currencyType, CancellationToken cancellationToken  = default)
    {
        var cacheFiles = GetCurrencyCacheFiles(_baseCurrency);

        if (!cacheFiles.Any())
        {
            return await FetchAndCacheCurrentCurrency(currencyType, cancellationToken);
        }

        var latestFile = cacheFiles
            .OrderByDescending(f => ExtractDateTimeFromFileName(f))
            .First();

        var fileDateTime = ExtractDateTimeFromFileName(latestFile);

        if (DateTime.Now - fileDateTime > _cacheExpirtation)
        {
            return await FetchAndCacheCurrentCurrency(currencyType, cancellationToken);
        }

        return await GetCurrencyFromCache(latestFile, currencyType);
    }

    public async Task<CurrencyExchangeRate> GetCurrencyOnDateAsync(string currencyType, DateOnly date, CancellationToken cancellationToken = default)
    {
        var dateFiles = GetCurrencyCacheFiles(_baseCurrency)
            .Where(f => ExtractDateFromFileName(f) == date)
            .ToList();

        if (!dateFiles.Any())
        {
            return await FetchAndCacheCurrencyOnDate(currencyType, date, cancellationToken);
        }

        var latestFile = dateFiles
            .OrderByDescending(f => ExtractDateTimeFromFileName(f))
            .First();

        return await GetCurrencyFromCache(latestFile, currencyType);
    }
}
