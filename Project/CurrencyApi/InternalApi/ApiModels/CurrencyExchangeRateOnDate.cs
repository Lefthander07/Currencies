namespace Fuse8.BackendInternship.InternalApi.ApiModels;

/// <summary>
/// Курсы валют на определенную дату.
/// </summary>
public record CurrencyExchangeRateOnDate
{
    /// <summary>
    /// Дата последнего обновления курса.
    /// </summary>
    public required DateTime LastUpdatedAt { get; init; }

    /// <summary>
    /// Массив курсов валют.
    /// </summary>
    public required CurrencyExchangeRate[] Currencies { get; init; }
}

