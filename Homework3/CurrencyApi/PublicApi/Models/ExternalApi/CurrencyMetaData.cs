using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models.ExternalApi;

/// <summary>
/// Метадата с внешнего API, хранящая дату актуальности полученного курса валют
/// </summary>
public record MetaData
{
    /// <summary>
    /// Время последнего обновления данных в формате строки.
    /// </summary>
    [JsonPropertyName("last_updated_at")]
    public required string LastUpdatedAt { get; init; }
}
