namespace Core.DTOs.Common;

/// <summary>
/// Bulk operation error detail
/// </summary>
public class BulkOperationError
{
    /// <summary>
    /// Entity ID that failed
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}