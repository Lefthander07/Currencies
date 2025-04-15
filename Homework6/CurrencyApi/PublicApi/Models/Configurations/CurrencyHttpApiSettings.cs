using System.ComponentModel.DataAnnotations;

namespace Fuse8.BackendInternship.PublicApi.Models.Configurations;

/// <summary>
/// Модель конфига приложения
/// </summary>
public sealed record CurrencyHttpApiSettings
{
    /// <summary>
    /// Авторизационный ключ
    /// </summary>
    [Required(ErrorMessage = "Базовый ключ доступа не установлен")]
    public required string ApiKey { get; init; }

    /// <summary>
    /// Базовый урл API
    /// </summary>
    [Required(ErrorMessage = "Базовый URL не установлен")]
    public required string BaseUrl { get; init; }
}
