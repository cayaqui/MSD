namespace Core.DTOs.Common;

/// <summary>
/// Base DTO class for entities
/// </summary>
public abstract class BaseDto
{
    /// <summary>
    /// Entity ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
