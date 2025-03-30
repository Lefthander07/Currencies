using Fuse8.BackendInternship.PublicApi.Models.Responses;
using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models.ExternalApi;

/// <summary>
/// Ответ с внешнего API
/// </summary>
public record CurrencyApiResponse
{
    /// <summary>
    /// Метаданные, связанные с ответом API.
    /// </summary>
    [JsonPropertyName("meta")]
    public required MetaData Meta { get; set; }

    /// <summary>
    /// Данные о валюте, где ключ - код валюты, а значение - информация о курсе.
    /// </summary>
    [JsonPropertyName("data")]
    public required Dictionary<string, CurrencyCurrentResponse> Data { get; set; }
}
