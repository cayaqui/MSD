namespace Core.DTOs.Common;

/// <summary>
/// Bulk operation result
/// </summary>
public class BulkOperationResult
{
    public BulkOperationResult()
    {
        
    }
    public BulkOperationResult(int success, int failure)
    {
        SuccessCount = success;
        FailureCount = failure;
    }
    /// <summary>
    /// Number of successful operations
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of failed operations
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Total number of operations
    /// </summary>
    public int TotalCount => SuccessCount + FailureCount;

    /// <summary>
    /// List of failed IDs with error messages
    /// </summary>
    public List<BulkOperationError> Errors { get; set; } = new();
}
