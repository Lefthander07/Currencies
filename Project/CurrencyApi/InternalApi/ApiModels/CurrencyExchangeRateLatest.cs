using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.InternalApi.ApiModels
{
    /// <summary>
    /// Курс валют на текущую дату
    /// </summary>
    public record CurrencyExchangeRateLatest
    {
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
}
