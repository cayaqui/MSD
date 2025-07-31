namespace Core.DTOs.Common;

/// <summary>
/// Bulk operation DTO
/// </summary>
public class BulkOperationDto
{
    /// <summary>
    /// IDs of entities to operate on
    /// </summary>
    public List<Guid> Ids { get; set; } = new();
}
