using Fuse8.BackendInternship.gRPC;
namespace Fuse8.BackendInternship.Extensions;

public static class CurrencyCodeEnumExtensions
{
    public static string ToUpperString(this CurrencyCode currencyCode)
    {
        return currencyCode.ToString().ToUpper();
    }
    
    public static CurrencyCode ToCurrencyCodeEnum(this string currencyCode)
    {
        if (Enum.TryParse(currencyCode, true, out CurrencyCode parsed) && parsed != CurrencyCode.NotSet)
            return parsed;

        throw new ArgumentException($"Invalid currency code: {currencyCode}");
    }

    public static string ToStringCode(this CurrencyCode code)
    {
        return code.ToUpperString();
    }
}
