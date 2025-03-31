using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.InternalApi.Configurations;
public sealed record Ports
{

    /// <summary>
    /// Валюта по умолчанию для получения курса.
    /// </summary>
    [JsonPropertyName("REST")]
    [Required(ErrorMessage = "Валюта по умолчанию не установлена")]
    public required string REST { get; init; }

    /// <summary>
    /// Базовая валюта, относительно которой будет рассчитываться курс.
    /// </summary>
    [JsonPropertyName("gRPC")]
    [Required(ErrorMessage = "Базовая валюта не установлена")]
    public required string gRPC { get; init; }
}
