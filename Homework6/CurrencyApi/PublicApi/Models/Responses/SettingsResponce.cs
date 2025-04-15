using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models.Responses;

/// <summary>
/// Настройки приложения и статус с внешнего API
/// </summary>
public record SettingResponse
{
    [JsonPropertyName("defaultCurrency")]
    public required string DefaultCurrency { get; init; }

    [JsonPropertyName("newRequestsAvailable")]
    public required bool NewRequestsAvailable { get; init; } 

    [JsonPropertyName("currencyRoundCount")]
    public required int CurrencyRoundCount { get; init; }
}