namespace Fuse8.BackendInternship.Domain;
public static class DomainExtensions
{
	public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
	{
		return (value == null || !value.Any()) ;
	}

    public static string JoinToString<T>(this IEnumerable<T> value, string separator)
	{
		if (value.IsNullOrEmpty())
		{
			throw new ArgumentNullException(nameof(value));
		}

		return string.Join(separator, value);
	}

	public static int DaysCountBetweem(this DateTimeOffset StartDate, DateTimeOffset EndDate)
	{
		return (EndDate - StartDate).Days;
	}
}