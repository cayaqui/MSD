using Core.DTOs.Common;

namespace Core.DTOs.Projects.WBSElement;

/// <summary>
/// Alias for ImportResultDto used in WBS operations
/// </summary>
public class ImportResult : ImportResultDto
{
    public ImportResult() : base() { }
    
    public ImportResult(int successCount, int failedCount, List<string> errors)
    {
        Success = failedCount == 0;
        SuccessfulRecords = successCount;
        FailedRecords = failedCount;
        TotalRecords = successCount + failedCount;
        Errors = errors.ToArray();
    }
}