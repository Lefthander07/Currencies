using Fuse8.BackendInternship.InternalApi.Configurations;
using Fuse8.BackendInternship.InternalApi.Contracts;
using Fuse8.BackendInternship.InternalApi.Exceptions;
using Fuse8.BackendInternship.Exceptions;
using Fuse8.BackendInternship.InternalApi.ApiModels;
using Microsoft.Extensions.Options;


public sealed class CurrencyHttpApi : ICurrencyAPI
{
    public const string AuthorizationHeaderName = "apikey";
    private readonly HttpClient _httpClient;

    public CurrencyHttpApi(HttpClient httpClient, IOptionsSnapshot<CurrencyHttpApiOptions> apiSettingsSnapshot)
    {
        _httpClient = httpClient;
        var currencyHttpApiSettings = apiSettingsSnapshot.Value;

        _httpClient.BaseAddress = new Uri(currencyHttpApiSettings.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add(AuthorizationHeaderName, currencyHttpApiSettings.ApiKey);
    }

    /// <summary>
    /// Получает список всех актуальных курсов валют по отношению к указанной базовой валюте.
    /// Данные запрашиваются с внешнего API (endpoint: latest).
    /// </summary>
    /// <param name="baseCurrency">Код базовой валюты (например, "USD"), относительно которой рассчитываются курсы.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Массив объектов <see cref="CurrencyExchangeRate"/>, содержащих коды валют и их текущие курсы.</returns>
    public Task<CurrencyExchangeRate[]> GetAllCurrentCurrenciesAsync(string baseCurrency, CancellationToken cancellationToken)
    {
        return GetCurrencyExchangeRateAsync(
            requestUrl: $"latest?&base_currency={baseCurrency}",
            cancellationToken);
    }

    /// <summary>
    /// Получает список всех курсов валют по отношению к указанной базовой валюте на заданную дату.
    /// Запрашивает данные с внешнего API, используя исторический endpoint.
    ///
    /// В случае ошибок валидации валюты выбрасывает исключение <see cref="CurrencyNotFoundException"/>.
    /// В случае других ошибок HTTP-запроса выбрасывает исключение <see cref="CurrencyHttpApiException"/>.
    /// </summary>
    /// <param name="baseCurrency">Код базовой валюты, относительно которой рассчитываются курсы.</param>
    /// <param name="date">Дата, на которую нужно получить курсы валют.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Объект <see cref="CurrencyExchangeRateOnDate"/>, содержащий список курсов валют на указанную дату.</returns>
    /// <exception cref="CurrencyHttpApiException">Выбрасывается, если запрос к API не удался или вернул неожиданный статус-код.</exception>
    /// <exception cref="CurrencyNotFoundException">Выбрасывается, если валюта не найдена в ответе API.</exception>

    public async Task<CurrencyExchangeRateOnDate> GetAllCurrenciesOnDateAsync(string baseCurrency, DateOnly date, CancellationToken cancellationToken)
    {
        await HasRemainApiRequestsAsync(cancellationToken);
        string requestUrl = $"historical?date={date:yyyy-MM-dd}&base_currency={baseCurrency}";
        var httpResponseMessage = await _httpClient.GetAsync(requestUrl, cancellationToken);

        await EnsureSucceedRequestAsync();
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<CurrencyRateApiResponse>(cancellationToken);
        if (response is null)
        {
            throw new CurrencyHttpApiException($"Не удалось получить {typeof(CurrencyRateApiResponse)} из HTTP-ответа");
        }

        return new CurrencyExchangeRateOnDate
        {
            LastUpdatedAt = response.Meta.LastUpdatedAt,
            Currencies = response.Data
                .Select(kvp => new CurrencyExchangeRate
                 {
                     CurrencyCode = kvp.Value.CurrencyCode,
                     Value = kvp.Value.Value
                 })
                 .ToArray()
        };


        async Task EnsureSucceedRequestAsync()
        {
            if (httpResponseMessage.StatusCode is System.Net.HttpStatusCode.OK)
            {
                return;
            }
            if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
            {
                var errorResponse = await httpResponseMessage.Content.ReadFromJsonAsync<CurrencyApiErrorResponse>();

                if (errorResponse?.Message == "Validation error" &&
                    errorResponse.Errors?.TryGetValue("currencies", out var currencyErrors) == true &&
                    currencyErrors.Contains("The selected currencies is invalid."))
                {
                    throw new CurrencyNotFoundException($"Неизвестная валюта");
                }
            }
            throw new CurrencyHttpApiException($"API вернул неожиданный HTTP-код {httpResponseMessage.StatusCode}");
        }
    }

    /// <summary>
    /// Проверяет, остались ли доступные запросы к API в текущем месяце.
    /// Если лимит запросов исчерпан, выбрасывает исключение <see cref="ApiRequestLimitException"/>.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <exception cref="ApiRequestLimitException">Выбрасывается, если исчерпан лимит запросов к API в текущем месяце.</exception>
    public async Task HasRemainApiRequestsAsync(CancellationToken cancellationToken)
    {
        var contentjson = await GetStatusAsync(cancellationToken);
        if (contentjson?.RateLimits?.MonthlyLimit?.Remaining == 0)
        {
            throw new ApiRequestLimitException("Request limit exceeded.");
        }
    }

    /// <summary>
    /// Запрашивает статус использования API, включая информацию о лимитах запросов.
    /// </summary>
    /// <param name="token">Токен отмены для асинхронной операции.</param>
    /// <returns>Возвращает объект <see cref="StatusApiResponse"/>, содержащий информацию о текущем статусе API.</returns>
    /// <exception cref="BadHttpRequestException">Выбрасывается, если ответ от API пуст или невалиден.</exception>
    /// <exception cref="ApiRequestLimitException">Выбрасывается, если исчерпан лимит запросов к API.</exception>
    public async Task<StatusApiResponse> GetStatusAsync(CancellationToken token = default)
    {
        var url = $"status";
        var response = await _httpClient.GetAsync(url, token);

        if (response.IsSuccessStatusCode)
        {
            var contentjson = await response.Content.ReadFromJsonAsync<StatusApiResponse>(token);
            return contentjson ?? throw new BadHttpRequestException("Ответ от API пуст или невалиден.");
        }
        throw new ApiRequestLimitException("Исчерпан лимит запросов");
    }

    /// <summary>
    /// Получает статус использования месячного лимита API и проверяет, не превышен ли лимит.
    /// </summary>
    /// <param name="token">Токен отмены для асинхронной операции.</param>
    /// <returns>Возвращает <see langword="true"/>, если оставшийся лимит на использование API больше нуля, иначе <see langword="false"/>.</returns>
    public async Task<bool> GetStatusUsedAsync(CancellationToken token = default)
    {
        var response = await GetStatusAsync(token);
        return response?.RateLimits?.MonthlyLimit?.Total > response?.RateLimits?.MonthlyLimit?.Used;
    }

    private async Task<CurrencyExchangeRate[]> GetCurrencyExchangeRateAsync(string requestUrl, CancellationToken cancellationToken)
    {
        await HasRemainApiRequestsAsync(cancellationToken);

        var httpResponseMessage = await _httpClient.GetAsync(requestUrl, cancellationToken);

        await EnsureSucceedRequestAsync();
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<CurrencyRateApiResponse>(cancellationToken);
        if (response is null)
        {
            throw new CurrencyHttpApiException($"Не удалось получить {typeof(CurrencyRateApiResponse)} из HTTP-ответа");
        }

       return response.Data.Values.Select(data => new CurrencyExchangeRate { CurrencyCode = data.CurrencyCode, Value = data.Value }).ToArray();

        async Task EnsureSucceedRequestAsync()
        {
            if (httpResponseMessage.StatusCode is System.Net.HttpStatusCode.OK)
            {
                return;
            }
            if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
            {
                var errorResponse = await httpResponseMessage.Content.ReadFromJsonAsync<CurrencyApiErrorResponse>();

                if (errorResponse?.Message == "Validation error" &&
                    errorResponse.Errors?.TryGetValue("currencies", out var currencyErrors) == true &&
                    currencyErrors.Contains("The selected currencies is invalid."))
                {
                    throw new CurrencyNotFoundException($"Неизвестная валюта");
                }
            }
            throw new CurrencyHttpApiException($"неожиданный HTTP-код {httpResponseMessage.StatusCode}");
        }
    }
}
