using Fuse8.BackendInternship.InternalApi.ApiModels;
using Fuse8.BackendInternship.InternalApi.Configurations;
using Fuse8.BackendInternship.InternalApi.Contracts;
using Fuse8.BackendInternship.InternalApi.Data;
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

        public CurrencyController(ICachedCurrencyAPI currencyService, CurrencyHttpApi currencyHttpApi, IOptionsSnapshot<CurrencyOptions> settings, CurrencyDbContext dbContext)
        {
            _currencyCachedService = currencyService;
        }

        /// <summary>
        /// Получить текущий курс валюты по её коду.
        /// </summary>
        /// <param name="sourceCurrency">целевая валюта, для которой рассчитывается курс.</param>
        /// <param name="baseCurrency"> базовая, относительно которой считается курс.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Объект, содержащий код валюты и её текущий курс.</returns>
        /// <response code="200">Возвращает текущий курс валюты, если запрос успешен.</response>
        /// <response code="404">Возвращает ошибку, если валюта с указанным кодом не найдена.</response>
        /// <response code="429">Возвращает ошибку, если превышен лимит запросов.</response>
        /// <response code="500">Возвращает ошибку, если произошла неизвестная ошибка на сервере.</response>
        [HttpGet]
        [ProducesResponseType(typeof(CurrencyExchangeRate), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<CurrencyExchangeRateLatest> GetCurrentCurrency(
            [FromQuery] string sourceCurrency, [FromQuery] string baseCurrency, CancellationToken cancellationToken)
        {

            var sourceCurrencyExchange = await _currencyCachedService.GetCurrentCurrencyAsync(sourceCurrency, cancellationToken);
            var baseCurrencyExchange = await _currencyCachedService.GetCurrentCurrencyAsync(baseCurrency, cancellationToken);

            var response = GetExchangeRateRelativeToEachOther(sourceCurrencyExchange, baseCurrencyExchange);

            return new CurrencyExchangeRateLatest
            {
                CurrencyCode = response.CurrencyCode,
                Value = response.Value
            };
        }

        /// <summary>
        /// Получить курс валюты на указанную дату.
        /// </summary>
        /// <param name="sourceCurrency">целевая валюта, для которой рассчитывается курс
        /// <param name="baseCurrency"/> базовая, относительно которой считается курс.</param>
        /// <param name="date">Дата, на которую нужно получить курс валюты.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Объект, содержащий код валюты, дату и её курс на указанную дату.</returns>
        /// <response code="200">Возвращает курс валюты на указанную дату, если запрос успешен.</response>
        /// <response code="400">Возвращает ошибку, если запрос был некорректным.</response>
        /// <response code="404">Возвращает ошибку, если валюта с указанным кодом или курс на указанную дату не найдены.</response>
        /// <response code="429">Возвращает ошибку, если превышен лимит запросов (слишком частые запросы).</response>
        /// <response code="500">Возвращает ошибку, если произошла неизвестная ошибка на сервере.</response>
        [HttpGet("{date}")]
        [ProducesResponseType(typeof(CurrencyExchangeRateHistorical), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CurrencyExchangeRateHistorical>> GetHistoricalCurrency(
            [FromQuery] string sourceCurrency,
            [FromQuery] string baseCurrency,
            [FromRoute] DateOnly date,
            CancellationToken cancellationToken)
        {
            var sourceCurrencyExchange = await _currencyCachedService.GetCurrencyOnDateAsync(sourceCurrency, date, cancellationToken);
            var baseCurrencyExchange = await _currencyCachedService.GetCurrencyOnDateAsync(baseCurrency, date, cancellationToken);

            var response = GetExchangeRateRelativeToEachOther(sourceCurrencyExchange, baseCurrencyExchange);

            return new CurrencyExchangeRateHistorical
            {
                Date = date,
                CurrencyCode = response.CurrencyCode,
                Value = response.Value
            };
        }

        private CurrencyExchangeRate GetExchangeRateRelativeToEachOther(CurrencyExchangeRate sourceCurrency, CurrencyExchangeRate baseCurrency)
        {
            return new CurrencyExchangeRate
            {
                CurrencyCode = sourceCurrency.CurrencyCode,
                Value = sourceCurrency.Value / baseCurrency.Value
            };
        }
    }
}