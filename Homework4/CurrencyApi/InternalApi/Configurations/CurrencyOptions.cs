using System.ComponentModel.DataAnnotations;

namespace Fuse8.BackendInternship.InternalApi.Configurations;
public sealed record CurrencyOptions
{
    /// <summary>
    /// Базовая валюта, относительно которой будет рассчитываться курс.
    /// </summary>
    [Required(ErrorMessage = "Базовая валюта не установлена.")]
    public required string BaseCurrency { get; init; }

    /// <summary>
    /// Время актуальности кэша.
    /// </summary>
    [Required(ErrorMessage = "Время актуальности кэша не установлено.")]
    public required int cacheExpiration { get; init; }
}
