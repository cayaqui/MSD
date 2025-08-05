namespace Core.Enums.Change;

/// <summary>
/// Approval decision types
/// </summary>
public enum ApprovalDecision
{
    Pending = 1,
    Approved = 2,
    ApprovedWithConditions = 3,
    Rejected = 4,
    Deferred = 5,
    Escalated = 6
}
