using Fuse8.BackendInternship.PublicApi.gRPC;
using Fuse8.BackendInternship.PublicApi.Models.Configurations;
using Fuse8.BackendInternship.PublicApi.Models.ExternalApi;
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
       // private readonly CurrencyService _currencyService;
        private readonly CurrencySettigns _configuration;
        private readonly CurrencyClient _currencyClient;

        public CurrencyController(CurrencyClient currencyClient, IOptionsSnapshot<CurrencySettigns> configuration)
        {
            _currencyClient = currencyClient;
            _configuration = configuration.Value;
        }

        private decimal RoundCurrencyValue(decimal value, int roundDigits)
        {
            return Math.Round(value, roundDigits);
        }

        /// <summary>
        /// Получение текущего курса валют.
        /// </summary>
        /// <returns>Объект с кодом валюты и её значением по отношению к базовой валюте.</returns>
        /// <response code="200">Возвращает курс валюты на указанную дату, если запрос успешен.</response>
        /// <response code="404">Возвращает ошибку, если передана некорректная валюта.</response>
        /// <response code="429">Возвращает ошибку, если число доступных запросов превысило лимит.</response>
        /// <response code="500">Возвращает ошибку, если произошла другая неизвестная ошибка.</response>
        [HttpGet]
        [ProducesResponseType(typeof(CurrencyCurrentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<CurrencyCurrentResponse> GetCurrencyRate(CancellationToken token)
        {
            string defaultCurrency = _configuration.DefaultCurrency;
            string baseCurrency = _configuration.BaseCurrency;
            var grpcResponse = await _currencyClient.GetCurrencyCurrent(defaultCurrency);

            int roundDigits = _configuration.CurrencyRoundCount;
            return new CurrencyCurrentResponse
            {
                Code = defaultCurrency,
                Value = RoundCurrencyValue(Convert.ToDecimal(grpcResponse.Value), roundDigits)
            };
        }

        /// <summary>
        /// Получение курса валюты по её коду.
        /// </summary>
        /// <param name="code">Код валюты, для которой нужно получить курс.</param>
        /// <returns>Объект с кодом валюты и её значением по отношению к базовой валюте.</returns>
        /// <response code="200">Возвращает курс валюты на указанную дату, если запрос успешен.</response>
        /// <response code="404">Возвращает ошибку, если передана некорректная валюта.</response>
        /// <response code="429">Возвращает ошибку, если число доступных запросов превысило лимит.</response>
        /// <response code="500">Возвращает ошибку, если произошла другая неизвестная ошибка.</response>
        [HttpGet("{code}")]
        [ProducesResponseType(typeof(CurrencyCurrentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<CurrencyCurrentResponse> GetCurrencyRateByCode([FromRoute] string code)
        {
            string defaultCurrency = code;
            string baseCurrency = _configuration.BaseCurrency;
            var grpcResponse = await _currencyClient.GetCurrencyCurrent(defaultCurrency);

            int roundDigits = _configuration.CurrencyRoundCount;
            return new CurrencyCurrentResponse
            {
                Code = defaultCurrency,
                Value = RoundCurrencyValue(Convert.ToDecimal(grpcResponse.Value), roundDigits)
            };
        }

        /// <summary>
        /// Получение курса валюты на определенную дату.
        /// </summary>
        /// <param name="code">Код валюты, для которой нужно получить курс.</param>
        /// <param name="date">Дата, на которую нужно получить курс.</param>
        /// <returns>Объект с кодом валюты, её значением на указанную дату.</returns>
        /// <response code="200">Возвращает курс валюты на указанную дату, если запрос успешен.</response>
        /// <response code="400">Возвращает ошибку, если формат даты не соответствует требуемому.</response>
        /// <response code="404">Возвращает ошибку, если передана некорректная валюта.</response>
        /// <response code="429">Возвращает ошибку, если число доступных запросов превысило лимит.</response>
        /// <response code="500">Возвращает ошибку, если произошла другая неизвестная ошибка.</response>
        [HttpGet("{code}/{date}")]
        [ProducesResponseType(typeof(CurrencyHistoricalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<CurrencyHistoricalResponse> GetCurrencyRateByDate([FromRoute] string code, [FromRoute] DateOnly date, CancellationToken token = default)
        {
            string defaultCurrency = code;
            string baseCurrency = _configuration.BaseCurrency;

            var grpcRessponse = await _currencyClient.GetCurrencyOnDateAsync(defaultCurrency, date.ToString());
            int roundDigits = _configuration.CurrencyRoundCount;
            
            return new CurrencyHistoricalResponse
            {
                Date = date.ToString("yyyy-MM-dd"),
                Code = defaultCurrency,
                Value = RoundCurrencyValue(Convert.ToDecimal(grpcRessponse.Value), roundDigits)
            };
        }
    }
}
