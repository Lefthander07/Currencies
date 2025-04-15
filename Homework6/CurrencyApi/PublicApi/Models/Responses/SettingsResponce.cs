using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models.Responses;

/// <summary>
/// Настройки приложения и статус с внешнего API
/// </summary>
public record SettingResponse
{
    [JsonPropertyName("default_currency")]
    public required string DefaultCurrency { get; init; }

    [JsonPropertyName("new_requests_available")]
    public required bool NewRequestsAvailable { get; init; } 

    [JsonPropertyName("currency_round_count")]
    public required int CurrencyRoundCount { get; init; }
}