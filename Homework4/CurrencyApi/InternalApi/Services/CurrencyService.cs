using Fuse8.BackendInternship.InternalApi.Configurations;
using Fuse8.BackendInternship.InternalApi.Contracts;
using Fuse8.BackendInternship.InternalApi.Exceptions;
using Fuse8.BackendInternship.InternalApi.ApiModels;
using Microsoft.Extensions.Options;


public sealed class CurrencyHttpApi : ICurrencyAPI
{
    public const string AuthorizationHeaderName = "apikey";
    private const string CurrenciesQueryKey = "currencies";
    private readonly HttpClient _httpClient;

    public CurrencyHttpApi(HttpClient httpClient, IOptionsSnapshot<CurrencyHttpApiSettings> apiSettingsSnapshot)
    {
        _httpClient = httpClient;
        var currencyHttpApiSettings = apiSettingsSnapshot.Value;

        _httpClient.BaseAddress = new Uri(currencyHttpApiSettings.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add(AuthorizationHeaderName, currencyHttpApiSettings.ApiKey);
    }

    public Task<CurrencyExchangeRate[]> GetAllCurrentCurrenciesAsync(string baseCurrency, CancellationToken cancellationToken)
    {
        return GetCurrencyExchangeRateAsync(
    requestUrl: $"latest?&base_currency={baseCurrency}",
    cancellationToken);
    }

    public async Task<CurrencyExchangeRateOnDate> GetAllCurrenciesOnDateAsync(string baseCurrency, DateOnly date, CancellationToken cancellationToken)
    {
        await HasRemainApiRequestsAsync(token: cancellationToken);
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

    public Task<CurrencyExchangeRate[]> GetAllCurrentesAsync(string baseCurrency, CancellationToken cancellationToken)
    {
        return GetCurrencyExchangeRateAsync(
    requestUrl: $"latest?$base_currency={baseCurrency}",
    cancellationToken);
    }

    public Task<CurrencyExchangeRate[]> GetCurrencyExchangeRateAsync(string currencyCode, string baseCurrency, CancellationToken cancellationToken)
    {
        return GetCurrencyExchangeRateAsync(
            requestUrl: $"latest?{CurrenciesQueryKey}={currencyCode}&base_currency={baseCurrency}",
            cancellationToken);
    }

    public Task<CurrencyExchangeRate[]> GetCurrencyExchangeRateOnDateAsync(string currencyCode, string baseCurrency, DateOnly date,
        CancellationToken cancellationToken)
    {
        return GetCurrencyExchangeRateAsync(
            requestUrl: $"historical?date={date:yyyy-MM-dd}&{CurrenciesQueryKey}={currencyCode}&base_currency={baseCurrency}",
            cancellationToken);
    }

    private async Task<CurrencyExchangeRate[]> GetCurrencyExchangeRateAsync(string requestUrl, CancellationToken cancellationToken)
    {
        await HasRemainApiRequestsAsync(token: cancellationToken);

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

    public async Task HasRemainApiRequestsAsync(CancellationToken token = default)
    {
        var contentjson = await GetStatusAsync(token);
        if (contentjson?.RateLimits?.MonthlyLimit?.Remaining == 0)
        {
            throw new ApiRequestLimitException("Request limit exceeded.");
        }
    }
    public async Task<StatusApiResponse> GetStatusAsync(CancellationToken token = default)
    {
        var url = $"status";
        var response = await _httpClient.GetAsync(url, token);

        if (response.IsSuccessStatusCode)
        {
            var contentjson = await response.Content.ReadFromJsonAsync<StatusApiResponse>(token);
            return contentjson ?? throw new BadHttpRequestException("Ответ от API пуст или невалиден.");
        }
        throw new BadHttpRequestException("Не удалось получить статус");
    }


}
