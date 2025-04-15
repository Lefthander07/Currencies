namespace Fuse8.BackendInternship.InternalApi.Data;

public class CurrencyExchange
{
    public int Id { get; set; }
    public int CurrencyCacheId { get; set; }
    public CurrencyCache CurrencyCache { get; set; }
    public string CurrencyCode { get; set; }
    public decimal ExchangeRate { get; set; }
}
