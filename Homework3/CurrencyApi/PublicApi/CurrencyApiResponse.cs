//using System.Text.Json.Serialization;
//namespace Fuse8.BackendInternship.PublicApi
//{
//    /// <summary>
//    /// Ответ от API валют с метаданными и данными о валюте.
//    /// </summary>
//    public class CurrencyApiResponse
//    {
//        /// <summary>
//        /// Метаданные, связанные с ответом API.
//        /// </summary>
//        public MetaData Meta { get; set; }

//        /// <summary>
//        /// Данные о валюте, где ключ - код валюты, а значение - информация о курсе.
//        /// </summary>
//        public Dictionary<string, CurrencyData> Data { get; set; }
//    }

//    /// <summary>
//    /// Метаданные, связанные с ответом API, такие как время последнего обновления.
//    /// </summary>
//    public class MetaData
//    {
//        /// <summary>
//        /// Время последнего обновления данных в формате строки.
//        /// </summary>
//        public string LastUpdatedAt { get; set; }
//    }

//    /// <summary>
//    /// Данные о валюте, включая дату, код и значение курса.
//    /// </summary>
//    public class CurrencyData
//    {
//        /// <summary>
//        /// Дата получения данных (если применимо). Это поле будет проигнорировано, если значение null.
//        /// </summary>
//        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
//        public string? Date { get; set; }

//        /// <summary>
//        /// Код валюты, например, USD, EUR.
//        /// </summary>
//        public string Code { get; set; }

//        /// <summary>
//        /// Значение валюты по отношению к базовой валюте.
//        /// </summary>
//        public decimal Value { get; set; }
//    }
//}

