using Fuse8.BackendInternship.PublicApi.Models.ExternalApi;
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
    public required string Date { get; init; }
}
