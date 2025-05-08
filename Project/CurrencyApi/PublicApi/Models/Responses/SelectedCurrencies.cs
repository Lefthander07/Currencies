using Fuse8.BackendInternship.PublicApi.Data;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models.Responses;

/// <summary>
/// Модель, представляющая данные об одной избранной валютной паре.
/// </summary>
public class SelectedCurrencies
{
    public SelectedCurrencies() { }
    public SelectedCurrencies(SelectedExchangeRate selectedExchangeRates)
    {
        Name = selectedExchangeRates.Name;
        CurrencyCode = selectedExchangeRates.CurrencyCode;
        BaseCurrency = selectedExchangeRates.BaseCurrency;
    }
    /// <summary>
    /// Уникальное имя валютной пары, заданное пользователем.
    /// </summary>
    [JsonPropertyName("name")]
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Код целевой валюты
    /// </summary>
    [JsonPropertyName("currency_code")]
    [Required]
    public string CurrencyCode { get; set; }

    /// <summary>
    /// Код базовой валюты, относительно которой считается курс
    /// </summary>
    [JsonPropertyName("base_currency")]
    [Required]
    public string BaseCurrency { get; set; }
}

