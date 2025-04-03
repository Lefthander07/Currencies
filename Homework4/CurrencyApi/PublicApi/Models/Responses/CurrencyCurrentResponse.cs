using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models.Responses;

/// <summary>
/// Курс валют текущий
/// </summary>
public record CurrencyCurrentResponse
{
    /// <summary>
    /// Код валюты, например, USD, EUR.
    /// </summary>
    [JsonPropertyName("code")]
    public required string Code { get; init; }

    /// <summary>
    /// Значение валюты по отношению к базовой валюте.
    /// </summary>
    [JsonPropertyName("value")]
    public required decimal Value { get; init; }
}