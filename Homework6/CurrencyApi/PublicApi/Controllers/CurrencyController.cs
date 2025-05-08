using Fuse8.BackendInternship.PublicApi.gRPC;
using Fuse8.BackendInternship.PublicApi.Models.Configurations;
using Fuse8.BackendInternship.PublicApi.Models.Core;
using Fuse8.BackendInternship.PublicApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Fuse8.BackendInternship.PublicApi.Controllers
{
    /// <summary>
    /// Контроллер для получения курса валют.
    /// </summary>
    [ApiController]
    [Route("currency")]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyOptions _configuration;
        private readonly CurrencyClient _currencyClient;
        private readonly CurrencyCodeDTO _defaultCurrency;
        private readonly CurrencyCodeDTO _baseCurrency;

        public CurrencyController(CurrencyClient currencyClient, IOptionsSnapshot<CurrencyOptions> configuration)
        {
            _currencyClient = currencyClient;
            _configuration = configuration.Value;
            _defaultCurrency = _configuration.DefaultCurrency;
            _baseCurrency = _configuration.BaseCurrency;
        }

        /// <summary>
        /// Получение текущего курса валют.
        /// </summary>
        /// <returns>Объект с кодом валюты и её значением по отношению к базовой валюте.</returns>
        /// <response code="200">Возвращает курс валюты на указанную дату, если запрос успешен.</response>
        /// <response code="500">Возвращает ошибку, если произошла другая неизвестная ошибка.</response>
        [HttpGet]
        [ProducesResponseType(typeof(CurrencyCurrentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<CurrencyCurrentResponse> GetCurrencyRate(CancellationToken cancellationToken)
        {
            var response = await _currencyClient.GetCurrencyCurrentAsync(_defaultCurrency, _baseCurrency, cancellationToken);

            return new CurrencyCurrentResponse
            {
                Code = response.CurrencyCode,
                Value = response.ExchangeRate
            };
        }

        /// <summary>
        /// Получение курса валюты по отношению к базовой.
        /// </summary>
        /// <param name="code">Код валюты, для которой нужно получить курс.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Объект с кодом валюты, её курсом и датой.</returns>
        /// <response code="200">Возвращает курс валюты на указанную дату.</response>
        /// <response code="500">Внутренняя ошибка сервера.</response>
        [HttpGet("{code}")]
        [ProducesResponseType(typeof(CurrencyCurrentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<CurrencyCurrentResponse> GetCurrencyRateByCodeAsync([FromRoute] CurrencyCodeDTO code,
                                                                              CancellationToken cancellationToken)
        {
            var response = await _currencyClient.GetCurrencyCurrentAsync(code, _baseCurrency, cancellationToken);

            return new CurrencyCurrentResponse
            {
                Code = response.CurrencyCode,
                Value = response.ExchangeRate
            };
        }

        /// <summary>
        /// Получение курса валюты по отношению к базовой на определённую дату.
        /// </summary>
        /// <param name="code">Код валюты, для которой нужно получить курс.</param>
        /// <param name="date">Дата, на которую необходимо получить курс.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Объект с кодом валюты, её курсом и датой.</returns>
        /// <response code="200">Возвращает курс валюты на указанную дату.</response>
        /// <response code="400">Некорректный запрос.</response>
        /// <response code="500">Внутренняя ошибка сервера.</response>
        [HttpGet("{code}/{date}")]
        [ProducesResponseType(typeof(CurrencyHistoricalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<CurrencyHistoricalResponse> GetCurrencyRateByDate([FromRoute] CurrencyCodeDTO code,
                                                                            [FromRoute] DateOnly date,
                                                                            CancellationToken cancellationToken)
        {
            var response = await _currencyClient.GetCurrencyOnDateAsync(code, _baseCurrency, date, cancellationToken);
            int roundDigits = _configuration.CurrencyRoundCount;

            return new CurrencyHistoricalResponse
            {
                Date = date,
                Code = response.CurrencyCode,
                Value = response.ExchangeRate
            };
        }
    }
}