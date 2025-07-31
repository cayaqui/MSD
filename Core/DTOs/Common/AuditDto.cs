namespace Core.DTOs.Common;

/// <summary>
/// DTO for audit information
/// </summary>
public class AuditDto : BaseDto
{
    /// <summary>
    /// User who created the entity
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// User who last updated the entity
    /// </summary>
    public string? UpdatedBy { get; set; }
}
