using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.JsonConverters;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string dateFormat = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Дата должна быть строковым типом");
        }

        var dateString = reader.GetString();

        if (!DateOnly.TryParseExact(dateString, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
        {
            throw new JsonException($"Формат даты неправильный: {dateString}. Небобходимый {dateFormat}");
        }
        
        return dateOnly;
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(dateFormat));
    }
}
