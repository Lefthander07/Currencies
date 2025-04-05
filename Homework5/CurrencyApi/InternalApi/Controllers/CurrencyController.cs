using Fuse8.BackendInternship.InternalApi.ApiModels;
using Fuse8.BackendInternship.InternalApi.Configurations;
using Fuse8.BackendInternship.InternalApi.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Fuse8.BackendInternship.InternalApi.Controllers
{
    /// <summary>
    /// Контроллер для получения курса валют.
    /// </summary>
    [ApiController]
    [Route("currency")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICachedCurrencyAPI _currencyCachedService;
        private readonly CurrencyHttpApi _currencyHttpApi;
        private readonly CurrencyOptions _settings;

        public CurrencyController(ICachedCurrencyAPI currencyService, CurrencyHttpApi currencyHttpApi, IOptionsSnapshot<CurrencyOptions> settings)
        {
            _currencyCachedService = currencyService;
            _currencyHttpApi = currencyHttpApi;
            _settings = settings.Value;
        }

        /// <summary>
        /// Получить текущий курс валюты
        /// </summary>
        [HttpGet("{currencyCode}")]
        [ProducesResponseType(typeof(CurrencyExchangeRate), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<CurrencyExchangeRateLatest> GetCurrentCurrency(
            [FromRoute] string currencyCode, CancellationToken cancellationToken)
        {
             var response =  await _currencyCachedService.GetCurrentCurrencyAsync(currencyCode, cancellationToken);
            return new CurrencyExchangeRateLatest
            {
                CurrencyCode = response.CurrencyCode,
                Value = response.Value
            };
        }

        /// <summary>
        /// Получить курс на дату
        /// </summary>
        [HttpGet("{currencyCode}/{date}")]
        [ProducesResponseType(typeof(CurrencyExchangeRateHistorical), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CurrencyExchangeRateHistorical>> GetHistoricalCurrency(
            [FromRoute] string currencyCode,
            [FromRoute] DateOnly date,
            CancellationToken cancellationToken)
        {
            var response =  await _currencyCachedService.GetCurrencyOnDateAsync(
                currencyCode,
                date,
                cancellationToken);
            return Ok(new CurrencyExchangeRateHistorical
            {
                Date = date,
                CurrencyCode = response.CurrencyCode,
                Value = response.Value
            });
        }

        /// <summary>
        /// Получить настройки
        /// </summary>
        [HttpGet("settings")]
        [ProducesResponseType(typeof(ApiStatus), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiStatus>> GetSettings(CancellationToken cancellationToken)
        {
            var response = await _currencyHttpApi.GetStatusUsedAsync(cancellationToken);
            return Ok(new ApiStatus {
                BaseCurrency = _settings.BaseCurrency,
                RequestsAvailable = response
            });
        }
    }
}