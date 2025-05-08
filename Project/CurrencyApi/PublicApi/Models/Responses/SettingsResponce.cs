using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models.Responses;

/// <summary>
/// Настройки приложения и статус с внешнего API
/// </summary>
public record SettingResponse
{
    /// <summary>
    /// Код валюты по умолчанию, например, "USD" или "EUR".
    /// </summary>
    [JsonPropertyName("default_currency")]
    public required string DefaultCurrency { get; init; }

    /// <summary>
    /// Флаг, указывающий, доступны ли новые запросы в API.
    /// </summary>
    [JsonPropertyName("new_requests_available")]
    public required bool NewRequestsAvailable { get; init; }

    /// <summary>
    /// Количество знаков после запятой для округления валютных значений.
    /// </summary>
    [JsonPropertyName("currency_round_count")]
    public required int CurrencyRoundCount { get; init; }
}