namespace Fuse8.BackendInternship.Domain;

/// <summary>
/// Контейнер для значения, с отложенным получением
/// </summary>
public class Lazy<TValue>
{
	// ToDo: Реализовать ленивое получение значение при первом обращении к Value

	public TValue? Value { get; }
}