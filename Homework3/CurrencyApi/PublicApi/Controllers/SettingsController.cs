using Fuse8.BackendInternship.PublicApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Fuse8.BackendInternship.PublicApi.Controllers
{
    /// <summary>
    /// Контроллер для работы с настройками приложения.
    /// </summary>
    [ApiController]
    [Route("settings")]
    public class Settings : ControllerBase
    {
        private readonly CurrencyService _currencyService;
        private readonly IOptionsSnapshot<CurrencySettings> _configuration;

        /// <summary>
        /// Конструктор для инъекции зависимостей.
        /// </summary>
        /// <param name="currencyService">Сервис для получения данных о курсах валют.</param>
        /// <param name="configuration">Настройки приложения.</param>
        public Settings(CurrencyService currencyService, IOptionsSnapshot<CurrencySettings> configuration)
        {
            _currencyService = currencyService;
            _configuration = configuration;
        }

        /// <summary>
        /// Получить текущие настройки приложения.
        /// </summary>
        /// <returns>Объект настроек приложения, включая текущие настройки валюты и информацию о запросах.</returns>
        /// <response code="200">Возвращает текущие настройки приложения.</response>
        /// <response code="400">Возвращает ошибку, если запрос не может быть обработан.</response>
        [HttpGet]
        public async Task<SettingsResponse> GetSettings()
        {
            // Получаем информацию о статусе запросов из внешнего API
            JsonResponse status = await _currencyService.getStatusAsync();

            return new SettingsResponse
            {
                DefaultCurrency = _configuration.Value.DefaultCurrency,  // Валюта по умолчанию
                BaseCurrency = _configuration.Value.BaseCurrency,      // Базовая валюта
                RequestLimit = status.Quotas.Month.Total,               // Лимит доступных запросов
                RequestCount = status.Quotas.Month.Used,                // Количество использованных запросов
                CurrencyRoundCount = _configuration.Value.CurrencyRoundCount // Количество знаков после запятой для валюты
            };
        }
    }

    /// <summary>
    /// Модель ответа с настройками приложения.
    /// </summary>
    public class SettingsResponse
    {
        /// <summary>
        /// Валюта по умолчанию.
        /// </summary>
        public string? DefaultCurrency { get; set; }

        /// <summary>
        /// Базовая валюта, относительно которой определяется курс.
        /// </summary>
        public string? BaseCurrency { get; set; }

        /// <summary>
        /// Лимит доступных запросов на месяц.
        /// </summary>
        public int RequestLimit { get; set; }

        /// <summary>
        /// Количество использованных запросов на месяц.
        /// </summary>
        public int RequestCount { get; set; }

        /// <summary>
        /// Количество знаков после запятой для валют.
        /// </summary>
        public int CurrencyRoundCount { get; set; }
    }
}
