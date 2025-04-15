using Fuse8.BackendInternship.InternalApi.ApiModels;
using Fuse8.BackendInternship.InternalApi.Contracts;
using Fuse8.BackendInternship.gRPC;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Fuse8.BackendInternship.Extensions;
namespace Fuse8.BackendInternship.InternalApi.gRPC;

public class CurrencyService : GrpcCurrency.GrpcCurrencyBase
{
    private readonly ICachedCurrencyAPI _currencyApi;
    private readonly CurrencyHttpApi _currencyHttpApi;

    public CurrencyService(ICachedCurrencyAPI currencyApi, CurrencyHttpApi currencyHttpApi)
    {
        _currencyApi = currencyApi;
        _currencyHttpApi = currencyHttpApi;
    }

    public override async Task<CurrencyRate> GetCurrencyCurrent(LatestCurrencyRequest request, ServerCallContext context)
    {
       var sourceCurrency = await _currencyApi.GetCurrentCurrencyAsync(request.CurrencyCode.ToStringCode(), context.CancellationToken);
       var baseCurrency = await _currencyApi.GetCurrentCurrencyAsync(request.BaseCurrency.ToStringCode(), context.CancellationToken);

        var response = GetExchangeRateRelativeToEachOther(sourceCurrency, baseCurrency);

        return new CurrencyRate
        {
            CurrencyCode = request.CurrencyCode,
            Value = response.Value
        };
    }

    public override async Task<CurrencyRate> GetCurrencyOnDate(HistoricalCurrencyRequest request, ServerCallContext context)
    {
        var grpcDateOnly = request.Date;
        var date = new DateOnly(grpcDateOnly.Year, grpcDateOnly.Month, grpcDateOnly.Day);

        var sourceCurrency = await _currencyApi.GetCurrencyOnDateAsync(request.CurrencyCode.ToStringCode(), date, context.CancellationToken);
        var baseCurrency = await _currencyApi.GetCurrencyOnDateAsync(request.BaseCurrency.ToStringCode(), date, context.CancellationToken);
        
        var response = GetExchangeRateRelativeToEachOther(sourceCurrency, baseCurrency);

        return new CurrencyRate
        {
            CurrencyCode = request.CurrencyCode,
            Value = response.Value
        };

    }

    public override async Task<Settings> GetSetting(Empty request, ServerCallContext context)
    {
        var response = await _currencyHttpApi.GetStatusUsedAsync(context.CancellationToken);
        return new Settings
        {
            RequestsAvailable = response
        };
    }

    private CurrencyExchangeRate GetExchangeRateRelativeToEachOther(CurrencyExchangeRate sourceCurrency, CurrencyExchangeRate baseCurrency)
    {
        return new CurrencyExchangeRate
        { 
            CurrencyCode = sourceCurrency.CurrencyCode,
            Value = sourceCurrency.Value / baseCurrency.Value 
        };
    }
}