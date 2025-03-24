namespace Fuse8.BackendInternship.PublicApi
{
    /// <summary>
    /// Настройки для работы с API валют.
    /// </summary>
    public class CurrencySettings
    {
        /// <summary>
        /// Ключ для авторизации в API валют.
        /// </summary>
        public string? API_KEY { get; set; }

        /// <summary>
        /// Валюта по умолчанию для получения курса.
        /// </summary>
        public string? DefaultCurrency { get; set; }

        /// <summary>
        /// Базовая валюта, относительно которой будет рассчитываться курс.
        /// </summary>
        public string? BaseCurrency { get; set; }

        /// <summary>
        /// Базовый URL для подключения к API валют.
        /// </summary>
        public string? BaseUrl { get; set; }

        /// <summary>
        /// Количество знаков после запятой, до которых следует округлять курс валют.
        /// </summary>
        public int CurrencyRoundCount { get; set; }
    }
}

