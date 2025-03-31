using Fuse8.BackendInternship.PublicApi.gRPC;
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
    private readonly CurrencyClient _currencyClient;
    private readonly CurrencySettigns _configuration;

    /// <summary>
    /// Конструктор для инъекции зависимостей.
    /// </summary>
    /// <param name="currencyClient">Сервис для получения данных о курсах валют.</param>
    /// <param name="configuration">Настройки приложения.</param>
    public Settings(CurrencyClient currencyClient, IOptionsSnapshot<CurrencySettigns> configuration)
    {
        _currencyClient = currencyClient;
        _configuration = configuration.Value;
    }

    /// <summary>
    /// Получить текущие настройки приложения.
    /// </summary>
    /// <returns>Объект настроек приложения, включая текущие настройки валюты и информацию о запросах.</returns>
    /// <response code="200">Возвращает текущие настройки приложения.</response>
    /// <response code="500">Возвращает ошибку, если запрос не может быть обработан.</response>
    [HttpGet]
    [ProducesResponseType(typeof(SettingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<SettingResponse> GetSettings()
    {
        // Получаем информацию о статусе запросов из внешнего API
        var status = await _currencyClient.GetSettingAsync();

        return new SettingResponse
        {
            DefaultCurrency = _configuration.DefaultCurrency,  
            BaseCurrency = status.BaseCurrency,
            NewRequestsAvailable = status.RequestsAvailable,
            CurrencyRoundCount = _configuration.CurrencyRoundCount 
        };
    }
}
