using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models.Configurations;

public record grpcUrlOptions
{
    /// <summary>
    /// Настройка для хранения url gRPC-сервера
    /// </summary>
    public required string Url { get; init;}
}
