namespace Fuse8.BackendInternship.InternalApi.Data;

public class CurrencyCache
{
    /// <summary>
    /// Идентифкатор кэша
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Базовая валюта, относительно которой строится кэш
    /// </summary>
    public string? BaseCurrency { get; set; }

    /// <summary>
    /// Дата актуальности кэша
    /// historical - то что прилетает с внешнего апи
    /// latest - текущее время
    /// </summary>
    public DateTime CacheDate { get; set; }

    /// <summary>
    /// Коллекция курсов влают
    /// </summary>
    public ICollection<CurrencyExchange>? ExchangeRates { get; set; }
}
