namespace Fuse8.BackendInternship.InternalApi.Exceptions;
public class CurrencyNotFoundException : Exception
{
    public CurrencyNotFoundException(string message) : base(message) { }
}
