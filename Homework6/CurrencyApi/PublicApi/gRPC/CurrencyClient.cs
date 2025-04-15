using Fuse8.BackendInternship.PublicApi.Models.Configurations;
using Fuse8.BackendInternship.PublicApi.Models.Core;
using Fuse8.BackendIntership.PublicApi.GrpcContracts;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;

namespace Fuse8.BackendInternship.PublicApi.gRPC;

public class CurrencyClient
{
    private readonly GrpcCurrency.GrpcCurrencyClient _grpcCurrencyClient;

    public CurrencyClient(GrpcCurrency.GrpcCurrencyClient grpcCurrencyClient, IOptionsSnapshot<CurrencySettigns> currencySettings)
    {  
        _grpcCurrencyClient = grpcCurrencyClient;
    }

    public async Task<CurrencyExchangeRate> GetCurrencyCurrentAsync(string CurrencyCode, string BaseCurrency, CancellationToken cancellationToken)
    {
        var request = new LatestCurrencyRequest
        {
            CurrencyCode = CurrencyCode,
            BaseCurrency = BaseCurrency        
        };
        var response = await _grpcCurrencyClient.GetCurrencyCurrentAsync(request);
        return new CurrencyExchangeRate
        {
            CurrencyCode = response.CurrencyCode,
            ExchangeRate = RoundCurrencyValue(response.Value, 2)
        };
    }

    public async Task<CurrencyExchangeRateOnDate> GetCurrencyOnDateAsync(string CurrencyCode, string BaseCurrency, DateOnly date, CancellationToken cancellationToken)
    {

        var request = new HistoricalCurrencyRequest
        {
            CurrencyCode = CurrencyCode,
            BaseCurrency = BaseCurrency,
            Date = date.ToString("yyyy-MM-dd")
        };

        var response =  await _grpcCurrencyClient.GetCurrencyOnDateAsync(request);
        return new CurrencyExchangeRateOnDate
        {
            CurrencyCode = response.CurrencyCode,
            ExchangeRate = RoundCurrencyValue(response.Value, 2),
            date = date.ToString("yyyy-MM-dd")
        };

    }

    public async Task<Settings> GetSettingAsync(CancellationToken cancellationToken)
    {

        return await _grpcCurrencyClient.GetSettingAsync(new Empty(), default, default, cancellationToken);
    }

    private decimal RoundCurrencyValue(double value, int roundDigits)
    {
        return (decimal)Math.Round(value, roundDigits);
    }
}
