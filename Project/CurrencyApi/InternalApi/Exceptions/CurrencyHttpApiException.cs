namespace Fuse8.BackendInternship.InternalApi.Exceptions;

/// <summary>
/// Исключение, которое выбрасывается при ошибках HTTP-запросов к API валют.
/// </summary>
public class CurrencyHttpApiException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="CurrencyHttpApiException"/> с указанным сообщением.
    /// </summary>
    /// <param name="message">Сообщение, описывающее исключение.</param>
    public CurrencyHttpApiException(string message) : base(message) { }
}