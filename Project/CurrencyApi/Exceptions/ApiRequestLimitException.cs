namespace Fuse8.BackendInternship.Exceptions;

/// <summary>
/// Исключение, которое выбрасывается, когда превышен лимит запросов к API.
/// </summary>
public class ApiRequestLimitException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ApiRequestLimitException"/> с указанным сообщением.
    /// </summary>
    public ApiRequestLimitException(string message) : base(message) { }
}
