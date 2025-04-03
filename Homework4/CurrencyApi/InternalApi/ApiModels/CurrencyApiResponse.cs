namespace Fuse8.BackendInternship.InternalApi.ApiModels;

using Fuse8.BackendInternship.InternalApi.Contracts;
using System.Text.Json.Serialization;

/// <summary>
/// Модель ответа API, содержащего курсы валют.
/// </summary>
public record CurrencyRateApiResponse
{
    /// <summary>
    /// Метаданные, содержащие дату последнего обновления.
    /// </summary>
    [JsonPropertyName("meta")]
    public required MetaData Meta { get; init; }

    /// <summary>
    /// Словарь с курсами валют, где ключ - код валюты.
    /// </summary>
    [JsonPropertyName("data")]
    public required Dictionary<string, CurrencyExchangeRate> Data { get; set; }
}

/// <summary>
/// Метаданные, содержащие дату последнего обновления курсов валют.
/// </summary>
public class MetaData
{
    /// <summary>
    /// Время последнего обновления данных.
    /// </summary>
    [JsonPropertyName("last_updated_at")]
    public DateTime LastUpdatedAt { get; set; }
}

/// <summary>
/// Представляет курс обмена валюты.
/// </summary>
public class CurrencyExchangeRate
{
    /// <summary>
    /// Код валюты (например, "USD", "EUR").
    /// </summary>
    [JsonPropertyName("code")]
    public required string CurrencyCode { get; set; }

    /// <summary>
    /// Значение обменного курса.
    /// </summary>
    [JsonPropertyName("value")]
    public decimal Value { get; set; }
}

