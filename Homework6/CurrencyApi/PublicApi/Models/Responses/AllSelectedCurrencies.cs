using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models.Responses;

/// <summary>
/// Модель ответа, содержащая массив всех избранных валют.
/// </summary>
public class AllSelectedCurrencies
{
    /// <summary>
    /// Массив всех избранных валют, добавленных пользователем.
    /// </summary>
    [JsonPropertyName("selected_currencies")]
    public required SelectedCurrencies[] SelectedCurrencies { get; set; }
}

