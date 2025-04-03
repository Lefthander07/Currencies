using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.InternalApi.ApiModels;
public record ApiStatus
{
    /// <summary>
    /// Базовая валюта API.
    /// </summary>
    [JsonPropertyName("base_currency")]
    public required string BaseCurrency { get; init; }

    /// <summary>
    /// Доступность запросов к API.
    /// </summary>
    [JsonPropertyName("requests_available")]
    public required bool RequestsAvailable { get; init; }
}
