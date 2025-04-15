namespace Fuse8.BackendInternship.PublicApi.Exceptions;
public class ApiRequestLimitException : Exception
{
    public ApiRequestLimitException(string message) : base(message) { }
}
