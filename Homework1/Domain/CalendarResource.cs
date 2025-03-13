namespace Fuse8.BackendInternship.Domain;

/// <summary>
/// Значения ресурсов для календаря
/// </summary>
public class CalendarResource
{
	public static readonly CalendarResource Instance = new();

    public static string January { get; private set; }
    public static string February { get; private set; }

    private static readonly string[] MonthNames;

	static CalendarResource()
	{
		MonthNames = new[]
		{
			"Январь",
			"Февраль",
			"Март",
			"Апрель",
			"Май",
			"Июнь",
			"Июль",
			"Август",
			"Сентябрь",
			"Октябрь",
			"Ноябрь",
			"Декабрь",
		};
		January = GetMonthByNumber(0);
		February = GetMonthByNumber(1);
	}

	private static string GetMonthByNumber(int number)
		=> MonthNames[number];

	//индексатор для получения названия месяца по енаму Month
	public string this[Month month]
	{
		get
		{
			if ((int)month > 11)
				throw new ArgumentOutOfRangeException($"Индекс месяца не может быть больше 11, Переданное значение: {(int)month}", nameof(month));

            if ((int)month < 0)
                throw new ArgumentOutOfRangeException($"Индекс месяца не может быть меньше, Переданное значение: {(int)month}", nameof(month));

            return GetMonthByNumber((int)month);
		}

	}
}

public enum Month
{
	January,
	February,
	March,
	April,
	May,
	June,
	July,
	August,
	September,
	October,
	November,
	December,
}