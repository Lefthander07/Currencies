namespace Fuse8.BackendInternship.PublicApi.Exceptions;
public class CurrencyNotFoundException : Exception
{
    public CurrencyNotFoundException(string message) : base(message) { }
}
