using System.Reflection;

namespace Fuse8.BackendInternship.Domain;

public static class BankCardHelpers
{
	/// <summary>
	/// Получает номер карты без маски
	/// </summary>
	/// <param name="card">Банковская карта</param>
	/// <returns>Номер карты без маски</returns>
	public static string GetUnmaskedCardNumber(BankCard card)
	{
		// ToDo: С помощью рефлексии получить номер карты без маски
		return card.MaskedCardNumber;
	}
}