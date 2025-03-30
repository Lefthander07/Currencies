using Fuse8.BackendInternship.PublicApi.Models.Configurations;
using Fuse8.BackendInternship.PublicApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Fuse8.BackendInternship.PublicApi.Controllers;

/// <summary>
/// Контроллер для работы с настройками приложения.
/// </summary>
[ApiController]
[Route("settings")]
public class Settings : ControllerBase
{
    private readonly CurrencyService _currencyService;
    private readonly CurrencySettigns _configuration;

    /// <summary>
    /// Конструктор для инъекции зависимостей.
    /// </summary>
    /// <param name="currencyService">Сервис для получения данных о курсах валют.</param>
    /// <param name="configuration">Настройки приложения.</param>
    public Settings(CurrencyService currencyService, IOptionsSnapshot<CurrencySettigns> configuration)
    {
        _currencyService = currencyService;
        _configuration = configuration.Value;
    }

    /// <summary>
    /// Получить текущие настройки приложения.
    /// </summary>
    /// <returns>Объект настроек приложения, включая текущие настройки валюты и информацию о запросах.</returns>
    /// <response code="200">Возвращает текущие настройки приложения.</response>
    /// <response code="500">Возвращает ошибку, если запрос не может быть обработан.</response>
    [HttpGet]
    [ProducesResponseType(typeof(SettingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<SettingsResponse> GetSettings()
    {
        // Получаем информацию о статусе запросов из внешнего API
        var status = await _currencyService.GetStatusAsync();

        return new SettingsResponse
        {
            DefaultCurrency = _configuration.DefaultCurrency,  // Валюта по умолчанию
            BaseCurrency = _configuration.BaseCurrency,      // Базовая валюта
            RequestLimit = status.RateLimits.MonthlyLimit.Total,               // Лимит доступных запросов
            RequestCount = status.RateLimits.MonthlyLimit.Used,                // Количество использованных запросов
            CurrencyRoundCount = _configuration.CurrencyRoundCount // Количество знаков после запятой для валюты
        };
    }
}
