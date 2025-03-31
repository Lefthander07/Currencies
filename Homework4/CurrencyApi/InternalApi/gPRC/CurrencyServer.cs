using Fuse8.BackendInternship.InternalApi.Configurations;
using Fuse8.BackendInternship.InternalApi.Contracts;
using Fuse8.BackendIntership.InternalApi.GrpcContracts;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;

namespace Fuse8.BackendInternship.InternalApi.gRPC;
public class CurrencyService : GrpcCurrency.GrpcCurrencyBase
{
    private readonly ICachedCurrencyAPI _currencyApi;
    private readonly CurrencyHttpApi _currencyService;
    private readonly CurrencySettigns _configuration;

    public CurrencyService(CashedCurrency currencyApi, CurrencyHttpApi currencyHttpApi, IOptionsSnapshot<CurrencySettigns> currencySettings)
    {
        _currencyApi = currencyApi;
        _currencyService = currencyHttpApi;
        _configuration = currencySettings.Value;
    }

    public override async Task<CurrencyRate> GetCurrencyCurrent(LatestCurrencyRequest request, ServerCallContext context)
    {
       var response = await _currencyApi.GetCurrentCurrencyAsync(request.CurrencyCode, CancellationToken.None);
        return new CurrencyRate { CurrencyCode = response.CurrencyCode,
            Value = (double)response.Value
        };
    }

    public override async Task<CurrencyRate> GetCurrencyOnDate(HistoricalCurrencyRequest request, ServerCallContext context)
    {
        var response = await _currencyApi.GetCurrencyOnDateAsync(request.CurrencyCode, DateOnly.Parse(request.Date), CancellationToken.None);
        return new CurrencyRate
        {
            CurrencyCode = response.CurrencyCode,
            Value = (double)response.Value
        };
    }

    public async override Task<Settings> GetSetting(Empty request, ServerCallContext context)
    {
        var response = await _currencyService.GetStatusAsync();

        return new Settings
        {
            BaseCurrency = _configuration.BaseCurrency,
            RequestsAvailable = response.RateLimits.MonthlyLimit.Total > response.RateLimits.MonthlyLimit.Remaining
        };
    }
}