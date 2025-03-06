using System.Data;
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
        if (kopeks < 0 || kopeks > 99)
            throw new ArgumentException("Копейки должны быть в диапазоне от 0 до 99.");

        if (rubles < 0)
            throw new ArgumentException("Рубли не могут быть отрицательными.");

        if (rubles == 0 && kopeks == 0 && isNegative)
            throw new ArgumentException("Невозможно создать отрицательную сумму с нулевыми рублями и копейками.");

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
		int lhsTotalKopeks = lhs.Rubles * 100 + lhs.Kopeks;
		int rhsTotalKopeks = rhs.Rubles * 100 + rhs.Kopeks;

        if (lhs.IsNegative)
            lhsTotalKopeks = -lhsTotalKopeks;

        if (rhs.IsNegative)
            rhsTotalKopeks = -rhsTotalKopeks;

        int Kopeks = lhsTotalKopeks + rhsTotalKopeks;

        bool negative = Kopeks < 0;

        int newRubles = System.Math.Abs(Kopeks) / 100;
        int newKopeks = System.Math.Abs(Kopeks) % 100;

        return new Money(negative, newRubles, newKopeks);
    }

    public static Money operator -(Money lhs, Money rhs)
    {
        //допустима логика lhs - rhs = lhs + (-rhs)
        Money newRhs;

        if (rhs.Rubles == 0 && rhs.Kopeks == 0)
        {
            newRhs = rhs;
        }
        else
        {
            newRhs = new Money(rhs.IsNegative ? false : true, rhs.Rubles, rhs.Kopeks);
        }

        return lhs + newRhs;

    }

    public static bool operator >(Money lhs, Money rhs)
    {
        int lhsTotals = lhs.Rubles * 100 + lhs.Kopeks;
        lhsTotals = lhs.IsNegative ? -lhsTotals : lhsTotals;

        int rhsTotals = rhs.Rubles * 100 + rhs.Kopeks;
        rhsTotals = rhs.IsNegative ? -rhsTotals : rhsTotals;

        return lhsTotals > rhsTotals;
    }

    public static bool operator >=(Money lhs, Money rhs)
    {
        int lhsTotals = lhs.Rubles * 100 + lhs.Kopeks;
        lhsTotals = lhs.IsNegative ? -lhsTotals : lhsTotals;

        int rhsTotals = rhs.Rubles * 100 + rhs.Kopeks;
        rhsTotals = rhs.IsNegative ? -rhsTotals : rhsTotals;

        return lhsTotals >= rhsTotals;
    }

    public static bool operator <(Money lhs, Money rhs)
    {
        int lhsTotals = lhs.Rubles * 100 + lhs.Kopeks;
        lhsTotals = lhs.IsNegative ? -lhsTotals : lhsTotals;

        int rhsTotals = rhs.Rubles * 100 + rhs.Kopeks;
        rhsTotals = rhs.IsNegative ? -rhsTotals : rhsTotals;

        return lhsTotals < rhsTotals;
    }

    public static bool operator <=(Money lhs, Money rhs)
    {
        int lhsTotals = lhs.Rubles * 100 + lhs.Kopeks;
        lhsTotals = lhs.IsNegative ? -lhsTotals : lhsTotals;

        int rhsTotals = rhs.Rubles * 100 + rhs.Kopeks;
        rhsTotals = rhs.IsNegative ? -rhsTotals : rhsTotals;

        return lhsTotals <= rhsTotals;
    }

    public static bool operator ==(Money lhs, Money rhs)
    {
        int lhsTotals = lhs.Rubles * 100 + lhs.Kopeks;
        lhsTotals = lhs.IsNegative ? -lhsTotals : lhsTotals;

        int rhsTotals = rhs.Rubles * 100 + rhs.Kopeks;
        rhsTotals = rhs.IsNegative ? -rhsTotals : rhsTotals;

        return lhsTotals == rhsTotals;
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
        int totalRubles = Rubles * 100 + Kopeks;
        totalRubles = this.IsNegative ? -totalRubles : totalRubles;
        return totalRubles.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Rubles},{Kopeks:D2}";
    }
}
