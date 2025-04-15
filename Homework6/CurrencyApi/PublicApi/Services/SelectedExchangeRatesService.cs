using Fuse8.BackendInternship.PublicApi.Data;

namespace Fuse8.BackendInternship.PublicApi.Services;

public class SelectedExchangeRatesService
{
    private readonly SelectedCurrenciesRepository _repository;
    private const int MaxCurrencyCodeLength = 5;
    private const int MaxNameLength = 100;

    public SelectedExchangeRatesService(SelectedCurrenciesRepository repository)
    {
        _repository = repository;
    }

    public Task<List<SelectedExchangeRate>> GetAll(CancellationToken cancellationToken)
        => _repository.GetAllAsync(cancellationToken);

    public Task<SelectedExchangeRate> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return GetExchangeRateExistsByName(name, cancellationToken);
    }

    public async Task<SelectedExchangeRate> CreateAsync(string sourceCurrency,
                                                         string baseCurrency,
                                                         string name,
                                                         CancellationToken cancellationToken)
    {
        CheckCurrencyCodeLength(sourceCurrency);
        CheckCurrencyCodeLength(baseCurrency);
        CheckSelectedExchangeRateConstraintLength(name);

        if (await _repository.ExistsByNameAsync(name, cancellationToken))
        {
            throw new InvalidOperationException($"Избранная валютная пара с именем '{name}' уже существует в базе данных");
        }

        if (await _repository.ExistsByCurrenciesAsync(sourceCurrency, baseCurrency, null, cancellationToken))
        {
            throw new InvalidOperationException($"Избранная валютная пара с {sourceCurrency} и {baseCurrency} существует.");
        }


        var newSelected = new SelectedExchangeRate
        {
            Name = name,
            CurrencyCode = sourceCurrency,
            BaseCurrency = baseCurrency
        };

        _repository.Add(newSelected);
        await _repository.SaveChangesAsync(cancellationToken);
        return newSelected;
    }

    public async Task DeleteAsync(string name, CancellationToken cancellationToken)
    {

        var existingByName = await GetExchangeRateExistsByName(name, cancellationToken);
        _repository.Remove(existingByName);
        await _repository.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateSelectedAsync(string name, string baseCurrency, string sourceCurrency, string newName, CancellationToken cancellationToken)
    {
        CheckCurrencyCodeLength(sourceCurrency);
        CheckCurrencyCodeLength(baseCurrency);
        CheckSelectedExchangeRateConstraintLength(name);

        var existingByName = await _repository.GetByNameAsync(name, cancellationToken);
        
        if (existingByName is null)
        {
            throw new InvalidOperationException($"Избранная валютная пара с именем '{name}' отсутствует в базе");
        }

        if (name != newName)
        {
            var existingByNewName = await _repository.ExistsByNameAsync(newName, cancellationToken);
            if (existingByNewName is true)
            {
                throw new InvalidOperationException($"Избранная валютная пара с новым именем '{name}' уже присутствует в базе");
            }
        }

        var existingByCurrencies = await _repository.ExistsByCurrenciesAsync(sourceCurrency, baseCurrency, name, cancellationToken);

        if (existingByCurrencies)
        {
            throw new InvalidOperationException($"Избранная валютная пара с {sourceCurrency} и {baseCurrency} существует.");
        }

        existingByName.CurrencyCode = sourceCurrency;
        existingByName.BaseCurrency = baseCurrency;
        existingByName.Name = newName;
        await _repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<SelectedExchangeRate> GetExchangeRateExistsByName(string name, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByNameAsync(name, cancellationToken);

        if (entity is null)
        {
            throw new InvalidOperationException($"Избранная валютная пара с именем '{name}' отсутствует в базе.");
        }

        return entity;
    }

    private bool CheckCurrencyCodeLength(string currencyCode)
    {
        return currencyCode.Length <= MaxCurrencyCodeLength 
            ? true 
            : throw new ArgumentException($"Код валюты '{currencyCode}' слишком длинный. Максимальная длина — {MaxCurrencyCodeLength} символов.");
    }

    private bool CheckSelectedExchangeRateConstraintLength(string name)
    {
        return name.Length <= MaxNameLength 
            ? true 
            : throw new ArgumentException($"Имя избранной пары слишком длинное. Максимальная длина — {MaxNameLength} символов.");
    }
}