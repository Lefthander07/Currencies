using Fuse8.BackendInternship.PublicApi.gRPC;
using Fuse8.BackendInternship.PublicApi.Models.Configurations;
using Fuse8.BackendInternship.PublicApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Fuse8.BackendInternship.PublicApi.Services;

namespace Fuse8.BackendInternship.PublicApi.Controllers;

/// <summary>
/// Контроллер для управления избранными валютными парами.
/// </summary>
[ApiController]
[Route("selected")]
public class SelectedCurrenciesController : ControllerBase
{
    private readonly SelectedExchangeRatesService _service;

    public SelectedCurrenciesController(SelectedExchangeRatesService service)
    {
            _service = service;
    }

    /// <summary>
    /// Получить список всех избранных валют пар.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список всех избранных валютных пар.</returns>
    /// <response code="200">Возвращает список избранных валютных, если запрос успешен.</response>
    /// <response code="500">Возвращает ошибку, если произошла другая неизвестная ошибка.</response>
    [HttpGet]
    [ProducesResponseType(typeof(AllSelectedCurrencies), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<AllSelectedCurrencies> GetAllSelectedAsync(CancellationToken cancellationToken)
    {
        var repsonse = await _service.GetAll(cancellationToken);

        return new AllSelectedCurrencies
        {
            SelectedCurrencies = repsonse.Select(p => new SelectedCurrencies(p)).ToArray()
        };
    }

    /// <summary>
    /// Получить информацию об избранной валюте по её названию.
    /// </summary>
    /// <param name="name">Название избранной валютной пары.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Информация об указанной валютной паре.</returns>
    /// <response code="200">Возвращает список избранных валютных, если запрос успешен.</response>
    /// <response code="500">Возвращает ошибку, если произошла другая неизвестная ошибка.</response>
    [HttpGet("{name}")]
    [ProducesResponseType(typeof(SelectedCurrencies), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<SelectedCurrencies> GetSelectedByName([FromRoute] string name, CancellationToken cancellationToken)
    {
        var repsonse = await _service.GetByNameAsync(name, cancellationToken);

        return new SelectedCurrencies(repsonse);
    }

    /// <summary>
    /// Добавить новую избранную валютную пару.
    /// </summary>
    /// <param name="request">Данные о валютной паре.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Добавленная валюта.</returns>
    /// <response code="200">Возвращает добавленную валютную пару, если операция успешна.</response>
    /// <response code="400">Некорректные входные данные.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpPost]
    [ProducesResponseType(typeof(SelectedCurrencies), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SelectedCurrencies>> CreateSelectedAsync([FromBody] SelectedCurrencies request, CancellationToken cancellationToken)
    {
        var response = await _service.CreateAsync(request.CurrencyCode,
                                                       request.BaseCurrency,
                                                       request.Name,
                                                       cancellationToken);

        return new SelectedCurrencies(response);
    }

    /// <summary>
    /// Удалить избранную валюту по названию.
    /// </summary>
    /// <response code="204">Валюта успешно удалена.</response>
    /// <response code="500">Возникла ошибка при удалении валюты.</response>
    [HttpDelete("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteByNameAsync([FromRoute] string name, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(name, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Обновить информацию об избранной валютной паре по названию.
    /// </summary>
    /// <param name="name">Текущее название валютной пары.</param>
    /// <param name="request">Новые данные.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат обновления.</returns>
    /// <response code="204">Валюта успешно обновлена.</response>
    /// <response code="400">Некорректные данные для обновления.</response>
    /// <response code="500">Возникла ошибка при обновлении валюты.</response>
    [HttpPut("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ChangeByNameAsync([FromRoute] string name,
                                                  [FromBody] SelectedCurrencies request,
                                                   CancellationToken cancellationToken)
    {
        await _service.UpdateSelectedAsync(name, request.BaseCurrency, request.CurrencyCode, request.Name, cancellationToken);
        return NoContent();

    }
}

