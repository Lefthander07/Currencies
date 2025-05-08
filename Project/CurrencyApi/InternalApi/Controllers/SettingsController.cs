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
    [Route("settings")]
    public class SettingsController : ControllerBase
    {
        private readonly CurrencyHttpApi _currencyHttpApi;

        public SettingsController(ICachedCurrencyAPI currencyService, CurrencyHttpApi currencyHttpApi, IOptionsSnapshot<CurrencyOptions> settings, CurrencyDbContext dbContext)
        {
            _currencyHttpApi = currencyHttpApi;
        }

        /// <summary>
        /// Получить настройки
        /// </summary>
        /// <returns>Текущий статус настроек API.</returns>
        /// <response code="200">Возвращает текущие настройки API, если запрос успешен.</response>
        /// <response code="500">Возвращает ошибку, если произошла неизвестная ошибка на сервере.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiStatus), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiStatus>> GetSettings(CancellationToken cancellationToken)
        {
            var response = await _currencyHttpApi.GetStatusUsedAsync(cancellationToken);
            return Ok(new ApiStatus
            {
                RequestsAvailable = response
            });
        }
    }
}