using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models.Responses;

/// <summary>
/// Настройки приложения и статус с внешнего API
/// </summary>
public record SettingsResponse
{
    /// <summary>
    /// Валюта по умолчанию.
    /// </summary>
    [JsonPropertyName("defaultCurrency")]
    public required string DefaultCurrency { get; init; }

    /// <summary>
    /// Базовая валюта, относительно которой определяется курс.
    /// </summary>
    [JsonPropertyName("baseCurrency")]
    public required string BaseCurrency { get; init; }

    /// <summary>
    /// Лимит доступных запросов на месяц.
    /// </summary>
    [JsonPropertyName("requestLimit")]
    public required int RequestLimit { get; init; }

    /// <summary>
    /// Количество использованных запросов на месяц.
    /// </summary>
    [JsonPropertyName("requestCount")]
    public required int RequestCount { get; init; }

    /// <summary>
    /// Количество знаков после запятой для валют.
    /// </summary>
    [JsonPropertyName("currencyRoundCount")]
    public required int CurrencyRoundCount { get; init; }
}