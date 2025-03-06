using System.Diagnostics.Metrics;
using System.Reflection;

namespace Fuse8.BackendInternship.Domain;

/// <summary>
/// Модель для хранения денег
/// </summary>
public class Money
{
	public Money(int rubles, int kopeks)
		: this(false, rubles, kopeks)
	{
	}

	public Money(bool isNegative, int rubles, int kopeks)
	{
		//Валидация входных параметров
		if (rubles < 0 || kopeks < 0 || kopeks > 99)
			throw new ArgumentException("Рубли не могут быть отрицательными, и копейки должны быть в диапазоне [0, 9]");
		IsNegative = isNegative;
		Rubles = rubles;
		Kopeks = kopeks;
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
		int rubles = lhs.Rubles + rhs.Rubles;
		int kopeks = lhs.Kopeks + rhs.Kopeks;

        if (kopeks >= 100)
        {
            kopeks -= 100;
            rubles += 1;
        }

        return new Money(rubles, kopeks);
    }

    public static Money operator -(Money lhs, Money rhs)
    {
        int rubles = lhs.Rubles - rhs.Rubles;
        int kopeks = lhs.Kopeks - rhs.Kopeks;

        if (kopeks < 0)
        {
            kopeks += 100;
            rubles -= 1;
        }

        return new Money(rubles, kopeks);
    }

    public static bool operator >(Money lhs, Money rhs)
    {
		return lhs.Rubles * 100 + lhs.Kopeks > rhs.Rubles * 100 + rhs.Kopeks;
    }

    public static bool operator >=(Money lhs, Money rhs)
    {
        return lhs.Rubles * 100 + lhs.Kopeks >= rhs.Rubles * 100 + rhs.Kopeks;
    }

    public static bool operator <(Money lhs, Money rhs)
    {
        return rhs > lhs;
    }

    public static bool operator <=(Money lhs, Money rhs)
    {
        return rhs >= lhs;
    }

    public static bool operator ==(Money lhs, Money rhs)
    {
        return lhs.Rubles * 100 + lhs.Kopeks == rhs.Rubles * 100 + rhs.Kopeks;
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
        return (Rubles * 100 + Kopeks).GetHashCode();
    }

    public override string ToString()
    {
        return $"{Rubles},{Kopeks:D2}";
    }
}
