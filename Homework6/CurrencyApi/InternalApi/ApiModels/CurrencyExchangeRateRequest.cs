namespace Fuse8.BackendInternship.InternalApi.ApiModels;

public class CurrencyExchangeRateRequest
{
    public required string SourceCurrency { get; set; }
    public required string BaseCurrency { get; set; }
}
