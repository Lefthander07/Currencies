using System.ComponentModel.DataAnnotations;

namespace Fuse8.BackendInternship.PublicApi.Models.Core;

/// <summary>
/// Модель курса валюты относительно базовой валюты
/// </summary>
public record CurrencyExchangeRate
{
    /// <summary>
    /// Код валюты, например, "rub".
    /// </summary>
    public required string CurrencyCode { get; init; }

    /// <summary>
    /// Курс валюты, например, 100.
    /// </summary>
    public required decimal ExchangeRate { get; init; }
}

