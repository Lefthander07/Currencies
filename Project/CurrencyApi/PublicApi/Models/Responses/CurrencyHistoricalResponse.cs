using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models.Responses;


/// <summary>
/// Курс валют с датой
/// </summary>
public record CurrencyHistoricalResponse : CurrencyCurrentResponse
{
    /// <summary>
    /// Момент на который курс был актуален
    /// </summary>
    [JsonPropertyName("date")]
    public required DateOnly Date { get; init; }
}
