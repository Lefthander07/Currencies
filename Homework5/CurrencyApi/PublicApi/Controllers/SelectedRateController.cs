using Fuse8.BackendInternship.PublicApi.gRPC;
using Fuse8.BackendInternship.PublicApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Fuse8.BackendInternship.PublicApi.Services;

namespace Fuse8.BackendInternship.PublicApi.Controllers;

/// <summary>
/// Контроллер для получения актуального и исторического курса избранной валютной пары.
/// </summary>
[ApiController]
[Route("selectedrate")]
public class SelectedRateController : ControllerBase
{
    private readonly SelectedExchangeRatesService _service;
    private readonly CurrencyClient _currencyClient;

    public SelectedRateController(SelectedExchangeRatesService service, CurrencyClient currencyClient)
    {
        _service = service;
        _currencyClient = currencyClient;
    }

    /// <summary>
    /// Получить актуальный курс выбранной валютной пары.
    /// </summary>
    /// <param name="name">Название выбранной валютной пары.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Актуальный курс валюты.</returns>
    /// <response code="200">Возвращает курс валютной пары на текущий момент, если запрос успешен.</response>
    /// <response code="404">Возвращает ошибку, если код валюты не найден</response>
    /// <response code="500">Возвращает ошибку, если произошла другая неизвестная ошибка.</response>
    [HttpGet("{name}")]
    [ProducesResponseType(typeof(CurrencyCurrentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<CurrencyCurrentResponse> GetSelectedRateLatest([FromRoute] string name, CancellationToken cancellationToken)
    {
        var selected = await _service.GetByNameAsync(name, cancellationToken);
        var response = await _currencyClient.GetCurrencyCurrentAsync(selected.CurrencyCode, selected.BaseCurrency, cancellationToken);

        return new CurrencyCurrentResponse
        {
            Code = response.CurrencyCode,
            Value = response.ExchangeRate
        };
    }

    /// <summary>
    /// Получить курс валютной пары по имени на указанную дату.
    /// </summary>
    /// <param name="name">Название выбранной валютной пары.</param>
    /// <param name="date">Дата, на которую нужно получить курс.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <response code="200">Возвращает курс валютной пары на определенную, если запрос успешен.</response>
    /// <response code="404">Возвращает ошибку, если код валюты не найден</response>
    /// <response code="500">Возвращает ошибку, если произошла другая неизвестная ошибка.</response>
    [HttpGet("{name}/{date}")]
    [ProducesResponseType(typeof(CurrencyCurrentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<CurrencyCurrentResponse> GetSelectedRateHistorical([FromRoute] string name, [FromRoute] DateOnly date, CancellationToken cancellationToken)
    {
        var selected = await _service.GetByNameAsync(name, cancellationToken);
        var response = await _currencyClient.GetCurrencyOnDateAsync(selected.CurrencyCode, selected.BaseCurrency, date, cancellationToken);
        return new CurrencyCurrentResponse
        {
            Code = response.CurrencyCode,
            Value = response.ExchangeRate
        };
    }
}

