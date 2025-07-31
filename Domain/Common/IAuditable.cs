namespace Domain.Common;

/// <summary>
/// Interface for entities with audit fields
/// </summary>
public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    string? CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
}
