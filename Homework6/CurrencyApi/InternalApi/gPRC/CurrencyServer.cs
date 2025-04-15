using Fuse8.BackendInternship.InternalApi.ApiModels;
using Fuse8.BackendInternship.InternalApi.Configurations;
using Fuse8.BackendInternship.InternalApi.Contracts;
using Fuse8.BackendIntership.InternalApi.GrpcContracts;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace Fuse8.BackendInternship.InternalApi.gRPC;
public class CurrencyService : GrpcCurrency.GrpcCurrencyBase
{
    private readonly ICachedCurrencyAPI _currencyApi;
    private readonly CurrencyHttpApi _currencyHttpApi;
    private readonly CurrencyOptions _configuration;

    public CurrencyService(ICachedCurrencyAPI currencyApi, CurrencyHttpApi currencyHttpApi, IOptionsSnapshot<CurrencyOptions> currencySettings)
    {
        _currencyApi = currencyApi;
        _currencyHttpApi = currencyHttpApi;
        _configuration = currencySettings.Value;
    }

    public override async Task<CurrencyRate> GetCurrencyCurrent(LatestCurrencyRequest request, ServerCallContext context)
    {
       var defaultCurrency = await _currencyApi.GetCurrentCurrencyAsync(request.CurrencyCode, context.CancellationToken);
       var baseCurrency = await _currencyApi.GetCurrentCurrencyAsync(request.BaseCurrency, context.CancellationToken);

        var response = GetExchangeRateRelativeToEachOther(defaultCurrency, baseCurrency);

        return new CurrencyRate
        {
            CurrencyCode = response.CurrencyCode,
            Value = (double)response.Value
        };
    }

    public override async Task<CurrencyRate> GetCurrencyOnDate(HistoricalCurrencyRequest request, ServerCallContext context)
    {
        var defaultCurrency = await _currencyApi.GetCurrencyOnDateAsync(request.CurrencyCode, DateOnly.ParseExact(request.Date, "yyyy-MM-dd"), context.CancellationToken);
        var baseCurrency = await _currencyApi.GetCurrencyOnDateAsync(request.BaseCurrency, DateOnly.ParseExact(request.Date, "yyyy-MM-dd"), context.CancellationToken);
        
        var response = GetExchangeRateRelativeToEachOther(defaultCurrency, baseCurrency);

        return new CurrencyRate
        {
            CurrencyCode = response.CurrencyCode,
            Value = (double)response.Value
        };
    }

    public async override Task<Settings> GetSetting(Empty request, ServerCallContext context)
    {
        var response = await _currencyHttpApi.GetStatusUsedAsync(context.CancellationToken);

        return new Settings
        {
            RequestsAvailable = response
        };
    }

    private CurrencyExchangeRate GetExchangeRateRelativeToEachOther(CurrencyExchangeRate defaultCurrency, CurrencyExchangeRate baseCurrency)
    {
        return new CurrencyExchangeRate
        { CurrencyCode = defaultCurrency.CurrencyCode,
        Value = defaultCurrency.Value / baseCurrency.Value };
    }
}