using System.ComponentModel.DataAnnotations;

namespace Fuse8.BackendInternship.InternalApi.Configurations;
public sealed record CurrencyOptions
{
    /// <summary>
    /// Время актуальности кэша.
    /// </summary>
    [Required(ErrorMessage = "Время актуальности кэша не установлено.")]
    public required int cacheExpiration { get; init; }
}
