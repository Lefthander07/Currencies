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

    /// <summary>
    /// Получает все выбранные обменные курсы из репозитория.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены, который позволяет отменить выполнение операции.</param>
    /// <returns>Список объектов <see cref="SelectedExchangeRate"/>, содержащий все выбранные обменные курсы.</returns>
    public Task<List<SelectedExchangeRate>> GetAll(CancellationToken cancellationToken)
        => _repository.GetAllAsync(cancellationToken);

    /// <summary>
    /// Получает выбранный обменный курс по имени из репозитория асинхронно.
    /// </summary>
    /// <param name="name">Имя выбранного обменного курса для поиска.</param>
    /// <param name="cancellationToken">Токен отмены, который позволяет отменить выполнение операции.</param>
    /// <returns>Объект <see cref="SelectedExchangeRate"/>, соответствующий найденному обменному курсу.</returns>
    public Task<SelectedExchangeRate> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return GetExchangeRateExistsByName(name, cancellationToken);
    }

    /// <summary>
    /// Создает новый выбранный обменный курс с заданными параметрами и сохраняет его в базе данных асинхронно.
    /// </summary>
    /// <param name="sourceCurrency">Код исходной валюты для обменного курса.</param>
    /// <param name="baseCurrency">Код базовой валюты для обменного курса.</param>
    /// <param name="name">Имя выбранной валютной пары.</param>
    /// <param name="cancellationToken">Токен отмены, который позволяет отменить выполнение операции.</param>
    /// <returns>Объект <see cref="SelectedExchangeRate"/>, представляющий созданную валютную пару.</returns>
    /// <exception cref="InvalidOperationException">
    /// Бросается, если выбранная валютная пара с указанным именем уже существует в базе данных или если
    /// выбранная валютная пара с указанными исходной и базовой валютами уже существует.
    /// </exception>
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

    /// <summary>
    /// Удаляет выбранный обменный курс по имени из базы данных асинхронно.
    /// </summary>
    /// <param name="name">Имя выбранного обменного курса для удаления.</param>
    /// <param name="cancellationToken">Токен отмены, который позволяет отменить выполнение операции.</param>
    /// <exception cref="KeyNotFoundException">Бросается, если обменный курс с указанным именем не найден в базе данных.</exception>
    public async Task DeleteAsync(string name, CancellationToken cancellationToken)
    {

        var existingByName = await GetExchangeRateExistsByName(name, cancellationToken);
        _repository.Remove(existingByName);
        await _repository.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Обновляет выбранный обменный курс с указанным именем, базовой валютой и исходной валютой на новое имя и валютные коды.
    /// </summary>
    /// <param name="name">Имя валютной пары для обновления.</param>
    /// <param name="baseCurrency">Новая базовая валюта для обменного курса.</param>
    /// <param name="sourceCurrency">Новая исходная валюта для обменного курса.</param>
    /// <param name="newName">Новое имя для выбранной валютной пары.</param>
    /// <param name="cancellationToken">Токен отмены, который позволяет отменить выполнение операции.</param>
    /// <exception cref="InvalidOperationException">
    /// Бросается, если выбранная валютная пара с указанным именем не существует в базе данных,
    /// если новое имя уже существует в базе или если обменная пара с указанными валютами уже существует.
    /// </exception>
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