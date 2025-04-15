namespace Fuse8.BackendInternship.Exceptions;

/// <summary>
/// Исключение, возникающее при попытке доступа к несуществующей валюте.
/// </summary>
public class CurrencyNotFoundException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="CurrencyNotFoundException"/> с указанным сообщением об ошибке.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    public CurrencyNotFoundException(string message) : base(message) { }
}