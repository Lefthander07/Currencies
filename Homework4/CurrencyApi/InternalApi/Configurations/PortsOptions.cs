using System.ComponentModel.DataAnnotations;

namespace Fuse8.BackendInternship.InternalApi.Configurations;
public sealed record PortsOptions
{

    /// <summary>
    /// Порт REST-API
    /// </summary>
    [Range(1, 65535)]
    public required int REST { get; init; }

    /// <summary>
    /// Порт gRPC-API
    /// </summary>
    [Range(1, 65535)]
    public required int gRPC { get; init; }
}
