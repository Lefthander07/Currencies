namespace Fuse8.BackendInternship.InternalApi.ApiModels;

using System.Text.Json.Serialization;

/// <summary>
/// Ответ от внешнего API, содержащий идентификатор аккаунта и информацию о лимитах запросов.
/// </summary>
public record StatusApiResponse
{
    /// <summary>
    /// Информация о квотах и лимитах на использование API.
    /// </summary>
    [JsonPropertyName("quotas")]
    public ApiRateLimits? RateLimits { get; init; }
}

/// <summary>
/// Информация о лимитах запросов к API, разделенная по периодам.
/// </summary>
public record ApiRateLimits
{
    /// <summary>
    /// Лимиты запросов на текущий месяц.
    /// </summary>
    [JsonPropertyName("month")]
    public ApiQuota? MonthlyLimit { get; init; }
}

/// <summary>
/// Детальная информация о квотах API: общее количество, использованные и оставшиеся запросы.
/// </summary>
public record ApiQuota
{
    /// <summary>
    /// Общее количество доступных запросов в периоде.
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; init; }

    /// <summary>
    /// Количество уже использованных запросов в периоде.
    /// </summary>
    [JsonPropertyName("used")]
    public int Used { get; init; }

    /// <summary>
    /// Количество оставшихся доступных запросов в периоде.
    /// </summary>
    [JsonPropertyName("remaining")]
    public int Remaining { get; init; }
}