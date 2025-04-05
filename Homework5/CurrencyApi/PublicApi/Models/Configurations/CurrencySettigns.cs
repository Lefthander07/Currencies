using System.ComponentModel.DataAnnotations;

namespace Fuse8.BackendInternship.PublicApi.Models.Configurations;
public sealed record CurrencySettigns
{

    /// <summary>
    /// Валюта по умолчанию для получения курса.
    /// </summary>
    [Required(ErrorMessage = "Валюта по умолчанию не установлена")]
    public required string DefaultCurrency { get; init; }

    /// <summary>
    /// Базовая валюта, относительно которой будет рассчитываться курс.
    /// </summary>
    [Required(ErrorMessage = "Базовая валюта не установлена")]
    public required string BaseCurrency { get; init; }


    /// <summary>
    /// Количество знаков после запятой, до которых следует округлять курс валют.
    /// </summary>
    [Required(ErrorMessage = "Количество знаков после запятой не установлено")]
    public required int CurrencyRoundCount { get; init; }
}
