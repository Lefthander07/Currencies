namespace Fuse8.BackendInternship.PublicApi.Models.Responses;
using System.Text.Json.Serialization;

/// <summary>
/// Модель, представляющая статус валютных данных в API.
/// </summary>
public record CurrencyStatus
{
    /// <summary>
    /// Валюта, используемая по умолчанию.
    /// </summary>
    [JsonPropertyName("defaultCurrency")]
    public required string DefaultCurrency { get; init; }

    /// <summary>
    /// Базовая валюта API.
    /// </summary>
    [JsonPropertyName("baseCurrency")]
    public required string BaseCurrency { get; init; }

    /// <summary>
    /// Флаг, указывающий на доступность новых запросов к API.
    /// </summary>
    [JsonPropertyName("newRequestsAvailable")]
    public required bool NewRequestsAvailable { get; init; }

    /// <summary>
    /// Количество знаков после запятой в значении валюты.
    /// </summary>
    [JsonPropertyName("currencyRoundCount")]
    public required int CurrencyRoundCount { get; init; }
}