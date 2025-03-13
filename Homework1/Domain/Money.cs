using System.Data;
using System.Diagnostics.Metrics;
using System.Reflection;
namespace Fuse8.BackendInternship.Domain;

/// <summary>
/// Модель для хранения денег
/// </summary>
public class Money
{
    private readonly int _totalKopeks;

    public Money(int rubles, int kopeks)
		: this(false, rubles, kopeks)
	{
	}

	public Money(bool isNegative, int rubles, int kopeks)
	{
        if (kopeks < 0)
            throw new ArgumentException($"Копейки не могут быть отрицательными. Переданное значение: {kopeks}.", nameof(kopeks));

        if (kopeks > 99)
            throw new ArgumentException($"Копейки не могут быть больше 99. Переданное значение: {kopeks}.", nameof(kopeks));

        if (rubles < 0)
            throw new ArgumentException("Рубли не могут быть отрицательными.");

        if (rubles == 0 && kopeks == 0 && isNegative)
            throw new ArgumentException("Невозможно создать отрицательную сумму с нулевыми рублями и копейками.");

        IsNegative = isNegative;
        Rubles = rubles;
        Kopeks = kopeks;

        // Рассчитываем общее количество копеек с учётом знака
        _totalKopeks = (isNegative ? -1 : 1) * (rubles * 100 + kopeks);
    }

	/// <summary>
	/// Отрицательное значение
	/// </summary>
	public bool IsNegative { get; }

	/// <summary>
	/// Число рублей
	/// </summary>
	public int Rubles { get; }

	/// <summary>
	/// Количество копеек
	/// </summary>
	public int Kopeks { get; }

    public static Money operator +(Money lhs, Money rhs)
    {
        int totalKopeks = lhs._totalKopeks + rhs._totalKopeks;
        bool isNegative = totalKopeks < 0;

        int newKopeks = Math.Abs(totalKopeks) % 100;
        int newRubles = Math.Abs(totalKopeks) / 100;

        return new Money(isNegative, newRubles, newKopeks);
    }

    public static Money operator -(Money lhs, Money rhs)
    {
        int totalKopeks = lhs._totalKopeks - rhs._totalKopeks;
        bool isNegative = totalKopeks < 0;

        int newKopeks = Math.Abs(totalKopeks) % 100;
        int newRubles = Math.Abs(totalKopeks) / 100;

        return new Money(isNegative, newRubles, newKopeks);

    }

    public static bool operator >(Money lhs, Money rhs)
    {
        return lhs._totalKopeks > rhs._totalKopeks;
    }

    public static bool operator >=(Money lhs, Money rhs)
    {
        return lhs._totalKopeks >= rhs._totalKopeks;
    }

    public static bool operator <(Money lhs, Money rhs)
    {
        return lhs._totalKopeks < rhs._totalKopeks;
    }

    public static bool operator <=(Money lhs, Money rhs)
    {
        return lhs._totalKopeks <= rhs._totalKopeks;
    }

    public static bool operator ==(Money lhs, Money rhs)
    {
        return lhs._totalKopeks == rhs._totalKopeks;
    }

    public static bool operator !=(Money lhs, Money rhs)
    {
        return !(lhs == rhs);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Money other)
        {
            return this == other;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return _totalKopeks.GetHashCode();
    }

    public override string ToString()
    {
        return $"{(IsNegative ? "-" : "")}{Rubles},{Kopeks:D2}";
    }
}
