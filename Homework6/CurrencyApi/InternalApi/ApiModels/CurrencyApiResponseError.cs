using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.InternalApi.ApiModels;

/// <summary>
/// Модель ответа API в случае ошибки.
/// </summary>
public record CurrencyApiErrorResponse
{
    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    /// <summary>
    /// Детализированные ошибки, сгруппированные по полям.
    /// </summary>
    [JsonPropertyName("errors")]
    public required Dictionary<string, List<string>> Errors { get; init; }

    ///// <summary>
    ///// Дополнительная информация об ошибке.
    ///// </summary>
    //[JsonPropertyName("info")]
    //public required string Info { get; init; }
}