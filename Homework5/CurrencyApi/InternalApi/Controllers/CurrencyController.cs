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
        /// <param name="currencyCode">Код валюты, для которой нужно получить текущий курс.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Объект, содержащий код валюты и её текущий курс.</returns>
        /// <response code="200">Возвращает текущий курс валюты, если запрос успешен.</response>
        /// <response code="404">Возвращает ошибку, если валюта с указанным кодом не найдена.</response>
        /// <response code="429">Возвращает ошибку, если превышен лимит запросов.</response>
        /// <response code="500">Возвращает ошибку, если произошла неизвестная ошибка на сервере.</response>
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
        /// Получить курс валюты на указанную дату.
        /// </summary>
        /// <param name="currencyCode">Код валюты, для которой нужно получить курс.</param>
        /// <param name="date">Дата, на которую нужно получить курс валюты.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Объект, содержащий код валюты, дату и её курс на указанную дату.</returns>
        /// <response code="200">Возвращает курс валюты на указанную дату, если запрос успешен.</response>
        /// <response code="400">Возвращает ошибку, если запрос был некорректным (например, неверный формат даты).</response>
        /// <response code="404">Возвращает ошибку, если валюта с указанным кодом или курс на указанную дату не найдены.</response>
        /// <response code="429">Возвращает ошибку, если превышен лимит запросов (слишком частые запросы).</response>
        /// <response code="500">Возвращает ошибку, если произошла неизвестная ошибка на сервере.</response>
        [HttpGet("{currencyCode}/{date}")]
        [ProducesResponseType(typeof(CurrencyExchangeRateHistorical), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
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
    }
}