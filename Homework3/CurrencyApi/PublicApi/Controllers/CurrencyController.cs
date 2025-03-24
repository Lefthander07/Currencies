using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Fuse8.BackendInternship.PublicApi.Controllers
{
    /// <summary>
    /// Контроллер для получения курса валют.
    /// </summary>
    [ApiController]
    [Route("currency")]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyService _currencyService;
        private readonly IOptionsSnapshot<CurrencySettings> _configuration;

        public CurrencyController(CurrencyService currencyService, IOptionsSnapshot<CurrencySettings> configuration)
        {
            _currencyService = currencyService;
            _configuration = configuration;
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
        public async Task<CurrencyData> GetCurrencyRate()
        {
            string defaultCurrency = _configuration.Value.DefaultCurrency;
            string baseCurrency = _configuration.Value.BaseCurrency;
            CurrencyApiResponse apiResponse = await _currencyService.GetCurrencyDataAsync(baseCurrency, defaultCurrency);

            int roundDigits = _configuration.Value.CurrencyRoundCount;
            return new CurrencyData
            {
                Code = defaultCurrency,
                Value = Math.Round(apiResponse.Data[defaultCurrency].Value, roundDigits)
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
        [HttpGet]
        [Route("{code}")]
        public async Task<CurrencyData> GetCurrencyRateByCode(string code)
        {
            string defaultCurrency = code;
            string baseCurrency = _configuration.Value.BaseCurrency;
            CurrencyApiResponse apiResponse = await _currencyService.GetCurrencyDataAsync(baseCurrency, defaultCurrency);

            int roundDigits = _configuration.Value.CurrencyRoundCount;
            return new CurrencyData
            {
                Code = defaultCurrency,
                Value = Math.Round(apiResponse.Data[defaultCurrency].Value, roundDigits)
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
        public async Task<CurrencyData> GetCurrencyRateByDate(string code, string date)
        {
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                throw new BadHttpRequestException($"Некорректная дата {date}. Формат yyyy-MM-dd");
            }

            string defaultCurrency = code;
            string baseCurrency = _configuration.Value.BaseCurrency;

            CurrencyApiResponse apiResponse = await _currencyService.GetCurrencyDataAsync(baseCurrency, defaultCurrency, parsedDate);
            int roundDigits = _configuration.Value.CurrencyRoundCount;
            
            return new CurrencyData
            {
                Date = date,
                Code = defaultCurrency,
                Value = Math.Round(apiResponse.Data[defaultCurrency].Value, roundDigits)
            };
        }
    }
}
