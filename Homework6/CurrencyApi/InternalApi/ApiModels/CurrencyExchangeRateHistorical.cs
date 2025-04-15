using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.InternalApi.ApiModels;

/// <summary>
/// Курс валют на определенную дату
/// </summary>
public record CurrencyExchangeRateHistorical
{
    /// <summary>
    /// Дата курса в формате DateOnly.
    /// </summary>
    [JsonPropertyName("date")]
    public required DateOnly Date { get; init; }

    /// <summary>
    /// Код валюты.
    /// </summary>
    [JsonPropertyName("code")]
    public required string CurrencyCode { get; init; }

    /// <summary>
    /// Значение курса валюты на указанную дату.
    /// </summary>
    [JsonPropertyName("value")]
    public decimal Value { get; init; }
}
