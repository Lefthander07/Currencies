using Fuse8.BackendInternship.InternalApi.ApiModels;
using Fuse8.BackendInternship.InternalApi.Configurations;
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
        private readonly CashedCurrency _currencyCachedService;
        private readonly CurrencyHttpApi _currencyHttpApi;
        private readonly CurrencySettigns _settings;

        public CurrencyController(CashedCurrency currencyService, CurrencyHttpApi currencyHttpApi, IOptionsSnapshot<CurrencySettigns> settings)
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
        public async Task<CurrencyExchangeRate> GetCurrentCurrency(
            [FromRoute] string currencyCode, CancellationToken cancellationToken)
        {
             var response =  await _currencyCachedService.GetCurrentCurrencyAsync(currencyCode, cancellationToken);
            return new CurrencyExchangeRate
            {
                CurrencyCode = response.CurrencyCode,
                Value = RoundCurrencyValue(response.Value, _settings.CurrencyRoundCount)

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
        public async Task<CurrencyExchangeRateHistorical> GetHistoricalCurrency(
            [FromRoute] string currencyCode,
            [FromRoute] string date)
        {
            var response =  await _currencyCachedService.GetCurrencyOnDateAsync(
                currencyCode,
                DateOnly.Parse(date),
                CancellationToken.None);
            return new CurrencyExchangeRateHistorical
            {
                Date = date,
                CurrencyCode = response.CurrencyCode,
                Value = RoundCurrencyValue(response.Value, _settings.CurrencyRoundCount)
            };
        }

        /// <summary>
        /// Получить настройки
        /// </summary>
        [HttpGet("settings")]
        [ProducesResponseType(typeof(ApiStatus), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ApiStatus> GetSettings()
        {
            var response = await _currencyHttpApi.GetStatusAsync();
            return new ApiStatus {
                BaseCurrency = _settings.BaseCurrency,
                RequestsAvailable = response?.RateLimits?.MonthlyLimit?.Remaining < response?.RateLimits?.MonthlyLimit?.Total
            };
        }
        private decimal RoundCurrencyValue(decimal value, int roundDigits)
        {
            return Math.Round(value, roundDigits);
        }
    }
}