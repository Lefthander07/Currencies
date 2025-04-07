namespace Fuse8.BackendInternship.PublicApi.Models.Responses;

/// <summary>
/// Модель, представляющая данные об одной избранной валютной паре.
/// </summary>
public class SelectedCurrencies
{
    /// <summary>
    /// Уникальное имя валютной пары, заданное пользователем.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Код целевой валюты
    /// </summary>
    public required string CurrencyCode { get; set; }

    /// <summary>
    /// Код базовой валюты, относительно которой считается курс
    /// </summary>
    public required string BaseCurrency { get; set; }
}

