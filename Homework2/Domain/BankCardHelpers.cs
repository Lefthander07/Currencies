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
		//С помощью рефлексии получам номер карты без маски
		const string NameOfField = "_number";

		FieldInfo? fieldInfo = typeof(BankCard).GetField(NameOfField, BindingFlags.Instance | BindingFlags.NonPublic);

		if (fieldInfo == null)
		{
			throw new InvalidOperationException("Поле _number отсутствует");

		}

		if (fieldInfo.GetValue(card) is not string UnmaskedCardNumber) {
			throw new InvalidOperationException("Поле _number null");
		}

		return UnmaskedCardNumber;
	}
}