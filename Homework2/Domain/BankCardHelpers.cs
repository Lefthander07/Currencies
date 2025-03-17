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
		FieldInfo? fieldInfo = typeof(BankCard).GetField("_number", BindingFlags.Instance | BindingFlags.NonPublic);

		if (fieldInfo == null)
		{
			throw new InvalidOperationException("Поле _number отсутствует");

		}

		string? UnmaskedCardNumber = fieldInfo.GetValue(card) as string;
		if (UnmaskedCardNumber == null) {
			throw new InvalidOperationException("Поле _number null");
		}

		return UnmaskedCardNumber;
	}
}