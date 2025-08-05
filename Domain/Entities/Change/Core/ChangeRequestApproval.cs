namespace Domain.Entities.Change.Core;

/// <summary>
/// Change Request Approval tracking
/// </summary>
public class ChangeRequestApproval : BaseEntity
{
    public Guid ChangeRequestId { get; private set; }
    public Guid ApproverId { get; private set; }
    public int ApprovalLevel { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime ApprovalDate { get; private set; }
    public string? Comments { get; private set; }

    public ChangeRequest ChangeRequest { get; private set; } = null!;
    public User Approver { get; private set; } = null!;

    private ChangeRequestApproval() { }

    public ChangeRequestApproval(
        Guid changeRequestId,
        Guid approverId,
        int approvalLevel,
        bool isApproved,
        string? comments = null
    )
    {
        ChangeRequestId = changeRequestId;
        ApproverId = approverId;
        ApprovalLevel = approvalLevel;
        IsApproved = isApproved;
        Comments = comments;
        ApprovalDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }
}
