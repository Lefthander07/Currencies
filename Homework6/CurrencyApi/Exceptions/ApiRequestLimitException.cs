namespace Fuse8.BackendInternship.Exceptions;
public class ApiRequestLimitException : Exception
{
    public ApiRequestLimitException(string message) : base(message) { }
}
