namespace Fuse8.BackendInternship.InternalApi.Data;

/// <summary>
/// Представляет обменный курс для конкретной валюты в контексте кэша валют.
/// </summary>
public class CurrencyExchange
{
    /// <summary>
    /// Уникальный идентификатор записи обменного курса.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор кэша валюты, к которому относится данный обменный курс.
    /// </summary>
    public int CurrencyCacheId { get; set; }

    /// <summary>
    /// Кэш валюты, к которому относится данный обменный курс.
    /// </summary>
    public CurrencyCache? CurrencyCache { get; set; }

    /// <summary>
    /// Код валюты, для которой задан обменный курс (например, "USD").
    /// </summary>
    public string? CurrencyCode { get; set; }

    /// <summary>
    /// Обменный курс для данной валюты относительно базовой валюты.
    /// </summary>
    public decimal ExchangeRate { get; set; }
}