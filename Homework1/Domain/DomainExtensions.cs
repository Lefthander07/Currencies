namespace Fuse8.BackendInternship.Domain;
public static class DomainExtensions
{
	public static bool IsNullOrEmpty<T>(this IEnumerable<T>? value)
	{
		return (value == null || !value.Any()) ;
	}

    public static string JoinToString<T>(this IEnumerable<T>? value, string separator)
	{
		if (value.IsNullOrEmpty())
		{
			throw new ArgumentNullException(nameof(value));
		}

		return string.Join(separator, value);
	}

	public static int DaysCountBetween(this DateTimeOffset StartDate, DateTimeOffset EndDate)
	{
		return Math.Abs((EndDate - StartDate).Days);
	}
}