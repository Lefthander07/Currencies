using System.Net;
using System.Net.Http;
using System.Text.Json;
using Fuse8.BackendInternship.PublicApi;
using Fuse8.BackendInternship.PublicApi.Exceptions;
using Fuse8.BackendInternship.PublicApi.Models.Configurations;
using Fuse8.BackendInternship.PublicApi.Models.ExternalApi;
using Microsoft.Extensions.Options;

public class CurrencyService
{
    private readonly HttpClient _httpClient;
    private readonly CurrencyHttpApiSettings _configurationAPI;


    public CurrencyService(HttpClient httpClient, IOptions<CurrencyHttpApiSettings> configurationAPI)
    {
        _httpClient = httpClient;
        _configurationAPI = configurationAPI.Value;


        _httpClient.BaseAddress = new Uri(_configurationAPI.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("apikey", _configurationAPI.ApiKey);
    }

    public async Task<CurrencyApiResponse> GetCurrencyDataAsync(string baseCurrency, string defaultCurrency, string? date = null, CancellationToken token = default)
    {
        var status = await HasRemainApiRequestsAsync(token);
        if (!status)
        {
            throw new ApiRequestLimitException("Request limit exceeded.");
        }

        string url;
        if (date is not null)
        {
            url = $"historical?currencies={defaultCurrency}&date={date:yyyy-MM-dd}&base_currency={baseCurrency}";
        }
        else
        {
            url = $"latest?currencies={defaultCurrency}&base_currency={baseCurrency}";
        }

        var response = await _httpClient.GetAsync(url, token);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<CurrencyApiErrorResponse>();

                if (errorResponse?.Message == "Validation error" &&
                    errorResponse.Errors?.TryGetValue("currencies", out var currencyErrors) == true &&
                    currencyErrors.Contains("The selected currencies is invalid."))
                {
                    throw new CurrencyNotFoundException($"Неизвестная валюта");
                }
            }
            throw new BadHttpRequestException("Ошибка получения данных");
        }

        string responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CurrencyApiResponse>(responseBody, new JsonSerializerOptions()) ?? throw new BadHttpRequestException("Ошибка получения данных");
    }

    public async Task<bool> HasRemainApiRequestsAsync(CancellationToken token = default)
    {
        var contentjson = await GetStatusAsync(token);
        return contentjson?.RateLimits?.MonthlyLimit?.Remaining > 0;
    }
    public async Task<StatusApiResponse> GetStatusAsync(CancellationToken token = default)
    {
        var url = $"status";
        var response = await _httpClient.GetAsync(url, token);

        if (response.IsSuccessStatusCode)
        {
            var contentjson = await response.Content.ReadFromJsonAsync<StatusApiResponse>();
            return contentjson;
        }
        throw new BadHttpRequestException("Не удалось получить статус");
    }
}