using System.Text.Json;
using Fuse8.BackendInternship.PublicApi;
using Microsoft.Extensions.Options;


public class CurrencyService
{
    private readonly HttpClient _httpClient;
    private readonly IOptionsSnapshot<CurrencySettings> _configuration;

    private readonly string _apiKey;
    private readonly string _baseUrl;

    public CurrencyService(HttpClient httpClient, IOptionsSnapshot<CurrencySettings> configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _apiKey = _configuration.Value.API_KEY;
        _baseUrl = _configuration.Value.BaseUrl;
        _configuration = configuration;
    }
    public async Task<CurrencyApiResponse> GetCurrencyDataAsync(string baseCurrency, string defaultCurrency, DateTime? date = null)
    {
        var status = await HasRemainApiRequestsAsync();
        if (!status)
        {
            throw new ApiRequestLimitException("Request limit exceeded.");
        }


        string url;
        if (date.HasValue)
        {
            url = $"{_baseUrl}/historical?currencies={defaultCurrency}&date={date.Value:yyyy-MM-dd}&base_currency={baseCurrency}&apikey={_apiKey}";
        }
        else
        {
            url = $"{_baseUrl}/latest?currencies={defaultCurrency}&base_currency={baseCurrency}&apikey={_apiKey}";
        }

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
            {
                throw new CurrencyNotFoundException("Неизвестная baseCurrency");
            }
            throw new BadHttpRequestException("Ошибка получения данных");
        }

        string responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CurrencyApiResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<bool> HasRemainApiRequestsAsync()
    {
        var contentjson = await getStatusAsync();
        return contentjson?.Quotas?.Month?.Remaining > 0;
    }
    public async Task<JsonResponse> getStatusAsync()
    {
        var url = $"{_baseUrl}/status?apikey={_apiKey}";
        Console.WriteLine(url);

        var response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var contentjson = await response.Content.ReadFromJsonAsync<JsonResponse>();
            return contentjson;
        }

        throw new BadHttpRequestException("Не удалось получить статус");
        
    }
}

public class Quotas
{
    public MonthQuota? Month { get; set; }
    public MonthQuota? Grace { get; set; }
}

public class MonthQuota
{
    public int Total { get; set; }
    public int Used { get; set; }
    public int Remaining { get; set; }
}

public class JsonResponse
{
    public long AccountId { get; set; }
    public Quotas? Quotas { get; set; }
}