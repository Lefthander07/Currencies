using Fuse8.BackendInternship.PublicApi.gRPC;
using Fuse8.BackendInternship.PublicApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Fuse8.BackendInternship.PublicApi.Services;
using Fuse8.BackendInternship.PublicApi.Models.Core;

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
    /// <param name="name">Название избранной валютной пары.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Актуальный курс валюты.</returns>
    /// <response code="200">Возвращает курс валютной пары на текущий момент, если запрос успешен.</response>
    /// <response code="500">Возвращает ошибку, если произошла другая неизвестная ошибка.</response>
    [HttpGet("{name}")]
    [ProducesResponseType(typeof(CurrencyCurrentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<CurrencyCurrentResponse> GetSelectedRateLatest([FromRoute] string name, CancellationToken cancellationToken)
    {
        var selected = await _service.GetByNameAsync(name, cancellationToken);

        CurrencyCodeDTO currencyCode = (CurrencyCodeDTO)Enum.Parse(typeof(CurrencyCodeDTO), selected.CurrencyCode);
        CurrencyCodeDTO baseCurrency = (CurrencyCodeDTO)Enum.Parse(typeof(CurrencyCodeDTO), selected.BaseCurrency);

        var response = await _currencyClient.GetCurrencyCurrentAsync(currencyCode, baseCurrency, cancellationToken);

        return new CurrencyCurrentResponse
        {
            Code = response.CurrencyCode,
            Value = response.ExchangeRate
        };
    }

    /// <summary>
    /// Получить курс валютной пары по имени на указанную дату.
    /// </summary>
    /// <param name="name">Название избранной валютной пары.</param>
    /// <param name="date">Дата, на которую нужно получить курс.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <response code="200">Возвращает курс валютной пары на определенную, если запрос успешен.</response>
    /// <response code="400">Возвращает ошибку, некорректный запрос (формат даты)</response>
    /// <response code="500">Возвращает ошибку, если произошла другая неизвестная ошибка.</response>
    [HttpGet("{name}/{date}")]
    [ProducesResponseType(typeof(CurrencyCurrentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CurrencyCurrentResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<CurrencyCurrentResponse> GetSelectedRateHistorical([FromRoute] string name, [FromRoute] DateOnly date, CancellationToken cancellationToken)
    {
        var selected = await _service.GetByNameAsync(name, cancellationToken);

        CurrencyCodeDTO currencyCode = (CurrencyCodeDTO)Enum.Parse(typeof(CurrencyCodeDTO), selected.CurrencyCode);
        CurrencyCodeDTO baseCurrency = (CurrencyCodeDTO)Enum.Parse(typeof(CurrencyCodeDTO), selected.BaseCurrency);

        var response = await _currencyClient.GetCurrencyOnDateAsync(currencyCode, baseCurrency, date, cancellationToken);
        return new CurrencyCurrentResponse
        {
            Code = response.CurrencyCode,
            Value = response.ExchangeRate
        };
    }
}

