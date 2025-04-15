using Fuse8.BackendInternship.PublicApi.Models.Configurations;
using Fuse8.BackendInternship.PublicApi.Models.Core;
using Fuse8.BackendInternship.gRPC;
using Fuse8.BackendInternship.Extensions;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using Enum = System.Enum;
using Fuse8.BackendInternship.Exceptions;

namespace Fuse8.BackendInternship.PublicApi.gRPC;

public class CurrencyClient
{
    private readonly GrpcCurrency.GrpcCurrencyClient _grpcCurrencyClient;
    private readonly CurrencyOptions _currencySettings;

    public CurrencyClient(GrpcCurrency.GrpcCurrencyClient grpcCurrencyClient, IOptionsSnapshot<CurrencyOptions> currencySettings)
    {  
        _grpcCurrencyClient = grpcCurrencyClient;
        _currencySettings = currencySettings.Value;
    }

    public async Task<CurrencyExchangeRate> GetCurrencyCurrentAsync(CurrencyCodeDTO сurrencyCodeFromRequest, CurrencyCodeDTO baseCurrencyFromRequest, CancellationToken cancellationToken)
    {
        var currencyCode = (CurrencyCode)сurrencyCodeFromRequest;
        if (currencyCode == default || Enum.IsDefined(currencyCode) is false)
        {
            throw new CurrencyNotFoundException($"Валюта с кодом {currencyCode} отсутствует");
        }

        var baseCurrency = (CurrencyCode)baseCurrencyFromRequest;
        if (baseCurrency == default || Enum.IsDefined(baseCurrency) is false)
        {
            throw new CurrencyNotFoundException($"Валюта с кодом {baseCurrency} отсутствует");
        }

        var request = new LatestCurrencyRequest
        {
            CurrencyCode = currencyCode,
            BaseCurrency = baseCurrency
        };

        var response = await _grpcCurrencyClient.GetCurrencyCurrentAsync(request, cancellationToken: cancellationToken);
        return new CurrencyExchangeRate
        {
            CurrencyCode = response.CurrencyCode.ToStringCode(),
            ExchangeRate = RoundCurrencyValue(response.Value)
        };
    }

    public async Task<CurrencyExchangeRateOnDate> GetCurrencyOnDateAsync(CurrencyCodeDTO сurrencyCodeFromRequest, CurrencyCodeDTO baseCurrencyFromRequest, DateOnly date, CancellationToken cancellationToken)
    {

        var currencyCode = (CurrencyCode)сurrencyCodeFromRequest;
        if (currencyCode == default || Enum.IsDefined(currencyCode) is false)
        {
            throw new CurrencyNotFoundException($"Валюта с кодом {currencyCode} отсутствует");
        }

        var baseCurrency = (CurrencyCode)baseCurrencyFromRequest;
        if (baseCurrency == default || Enum.IsDefined(baseCurrency) is false)
        {
            throw new CurrencyNotFoundException($"Валюта с кодом {baseCurrency} отсутствует");
        }

        var request = new HistoricalCurrencyRequest
        {
            CurrencyCode = currencyCode,
            BaseCurrency = baseCurrency,
            Date = new GRPCDateOnly { Year = date.Year, Month= date.Month, Day=date.Day }
        };

        var response =  await _grpcCurrencyClient.GetCurrencyOnDateAsync(request, cancellationToken: cancellationToken);
        return new CurrencyExchangeRateOnDate
        {
            CurrencyCode = response.CurrencyCode.ToStringCode(),
            ExchangeRate = RoundCurrencyValue(response.Value),
            date = date
        };
    }

    public async Task<Settings> GetSettingAsync(CancellationToken cancellationToken)
    {

        return await _grpcCurrencyClient.GetSettingAsync(new Empty(), cancellationToken: cancellationToken);
    }

    private decimal RoundCurrencyValue(decimal value)
    {
        return Math.Round(value, _currencySettings.CurrencyRoundCount);
    }
}
