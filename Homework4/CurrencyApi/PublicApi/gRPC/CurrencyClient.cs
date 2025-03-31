using Fuse8.BackendInternship.PublicApi.Models.Configurations;
using Fuse8.BackendInternship.PublicApi.Models.ExternalApi;
using Fuse8.BackendInternship.PublicApi.Models.Responses;
using Fuse8.BackendIntership.PublicApi.GrpcContracts;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;

namespace Fuse8.BackendInternship.PublicApi.gRPC;

public class CurrencyClient
{
    private readonly GrpcCurrency.GrpcCurrencyClient _grpcCurrencyClient;

    public CurrencyClient(GrpcCurrency.GrpcCurrencyClient grpcCurrencyClient, IOptionsSnapshot<CurrencySettigns> currencySettings)
    {  
        _grpcCurrencyClient = grpcCurrencyClient;;
    }

    public async Task<CurrencyRate> GetCurrencyCurrent(string CurrencyCode)
    {
        var request = new LatestCurrencyRequest { CurrencyCode = CurrencyCode };
        return await _grpcCurrencyClient.GetCurrencyCurrentAsync(request);
    }

    public async Task<CurrencyRate> GetCurrencyOnDateAsync(string CurrencyCode, string date)
    {
        var request = new HistoricalCurrencyRequest
        {
            CurrencyCode = CurrencyCode,
            Date = date
        };

        return await _grpcCurrencyClient.GetCurrencyOnDateAsync(request);
 
    }

    public async Task<Settings> GetSettingAsync()
    {

        return await _grpcCurrencyClient.GetSettingAsync(new Empty());
    }
}
