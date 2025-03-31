using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.InternalApi.Configurations;

/// <summary>
/// Модель для хранения данных с приватного файла конфигурации appseting.Development.Json
/// </summary>
public record CurrencyHttpApiSettings
{
    /// <summary>
    /// Ключ для внешнего API
    /// </summary>
    [JsonPropertyName("ApiKey")]
    public required string ApiKey { get; init; }

    /// <summary>
    /// Базовый URL для доступа к внешнему API
    /// </summary>
    [JsonPropertyName("BaseUrl")]
    public required string BaseUrl { get; init; }
}
