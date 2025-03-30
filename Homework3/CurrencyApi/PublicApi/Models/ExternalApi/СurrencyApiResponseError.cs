using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models.ExternalApi;
/// <summary>
/// Ответ на невалидный запрос
/// </summary>
public record CurrencyApiErrorResponse
{
    /// <summary>
    /// Текст ошибки
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    /// <summary>
    /// Список ошибок
    /// </summary>
    /// <value>
    /// Ключ - имя свойства, в котором ошибка
    /// Значение - спиоск ошибок
    /// </value>
    [JsonPropertyName("errors")]
    public required Dictionary<string, List<string>> Errors { get; init; }

    [JsonPropertyName("info")]
    public required string Info { get; init; }
}
