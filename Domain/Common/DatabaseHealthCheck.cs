namespace Domain.Common;

/// <summary>
/// Modelo para el resultado del health check de la base de datos
/// </summary>
public class DatabaseHealthCheck
{
    public bool IsHealthy { get; set; }
    public bool CanConnect { get; set; }
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    public bool HasPendingMigrations { get; set; }
    public List<string> PendingMigrations { get; set; } = new();
    public List<string> AppliedMigrations { get; set; } = new();
    public string DatabaseVersion { get; set; } = string.Empty;
    public string? Error { get; set; }
}