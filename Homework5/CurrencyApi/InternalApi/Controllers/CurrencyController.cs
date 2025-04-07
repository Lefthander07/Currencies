using Fuse8.BackendInternship.InternalApi.ApiModels;
using Fuse8.BackendInternship.InternalApi.Configurations;
using Fuse8.BackendInternship.InternalApi.Contracts;
using Fuse8.BackendInternship.InternalApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        private readonly CurrencyDbContext _dbContext;

        public CurrencyController(ICachedCurrencyAPI currencyService, CurrencyHttpApi currencyHttpApi, IOptionsSnapshot<CurrencyOptions> settings, CurrencyDbContext dbContext)
        {
            _currencyCachedService = currencyService;
            _currencyHttpApi = currencyHttpApi;
            _settings = settings.Value;
            _dbContext = dbContext;
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
                RequestsAvailable = response
            });
        }

        [HttpGet("test")]
        [ProducesResponseType(typeof(ApiStatus), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CurrencyExchangeRateLatest>> GetTest(CancellationToken cancellationToken)
        {
            var currencyType = "RUB";
            var _baseCurrency = "USD";


            var exchangeRate = await _dbContext.CurrencyCaches
                .Where(c => c.BaseCurrency == _baseCurrency &&
                            DateTime.UtcNow - c.CacheDate < TimeSpan.FromHours(2))
                .SelectMany(c => c.ExchangeRates)
                .Where(er => er.CurrencyCode == currencyType)
                .FirstOrDefaultAsync();

                if (exchangeRate is null)
                {
                    var currencies = await _currencyHttpApi.GetAllCurrentCurrenciesAsync(_baseCurrency, cancellationToken);
                    var cacheMeta = new CurrencyCache
                    { 
                        BaseCurrency = _baseCurrency, 
                        CacheDate = DateTime.UtcNow 
                    };
                    _dbContext.CurrencyCaches.Add(cacheMeta);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                
                    var currencyExchangeRates = currencies.Select(currency => new CurrencyExchange
                    {
                        CurrencyCacheId = cacheMeta.Id,
                        CurrencyCode = currency.CurrencyCode,
                        ExchangeRate = currency.Value
                    }).ToList();

                    _dbContext.CurrencyExchangeRates.AddRange(currencyExchangeRates);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    exchangeRate = currencyExchangeRates.FirstOrDefault(er => er.CurrencyCode == currencyType);
                }
            return new CurrencyExchangeRateLatest
            {
                CurrencyCode = exchangeRate.CurrencyCode,
                Value = exchangeRate.ExchangeRate
            };

        }
    }
}