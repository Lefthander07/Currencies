using System.ComponentModel.DataAnnotations;

namespace Fuse8.BackendInternship.PublicApi.Models.Core;

/// <summary>
/// Модель курса валюты относительно базовой валюты на определенную дату
/// </summary>
public record CurrencyExchangeRateOnDate
{
    /// <summary>
    /// Код валюты, например, "rub".
    /// </summary>
    [Required]
    public required string CurrencyCode { get; init; }

    /// <summary>
    /// Курс валюты, например, 100.
    /// </summary>
    [Required]
    public required decimal ExchangeRate { get; init; }

    /// <summary>
    /// Дата, на которую курс был актуален
    /// </summary>
    [Required]
    public required DateOnly date { get; init; }
}

