namespace Core.DTOs.Auth.Users;

public class BulkUserOperationDto
{
    public List<Guid> UserIds { get; set; } = new();
}
public record PaginationQuery(int PageNumber = 1, int PageSize = 10);
public record BulkUserOperationRequest(List<Guid> UserIds);
public record CanDeleteResult(bool CanDelete, string? Reason);
public record UniqueCheckQuery(Guid? ExcludeId);
public record InitializeResult(int Count, string Message);
public record ImportResult(int Count, string Message);

public class BulkAssignmentResult
{
    public int AssignedCount { get; }
    public int TotalCount { get; }
    public BulkAssignmentResult(int assignedCount, int totalCount)
    {
        AssignedCount = assignedCount;
        TotalCount = totalCount;
    }
}