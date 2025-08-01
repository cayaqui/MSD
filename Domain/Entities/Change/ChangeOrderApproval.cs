using System;
using Domain.Common;
using Domain.Entities.Security;
using Core.Enums.ChangeManagement;

namespace Domain.Entities.ChangeManagement;

/// <summary>
/// Change Order Approval tracking
/// </summary>
public class ChangeOrderApproval : BaseEntity, IAuditable
{
    // Basic Information
    public Guid ChangeOrderId { get; private set; }
    public string ApproverId { get; private set; } = string.Empty;
    public string ApprovalLevel { get; private set; } = string.Empty; // e.g., "Project Manager", "PMO", "Executive"
    public ApprovalDecision Decision { get; private set; }
    public DateTime DecisionDate { get; private set; }

    // Authority Limits
    public decimal AuthorityLimit { get; private set; }
    public bool IsWithinAuthority { get; private set; }

    // Decision Details
    public string? Comments { get; private set; }
    public string? Conditions { get; private set; }
    public bool IsConditional { get; private set; }

    // Navigation Properties
    public ChangeOrder ChangeOrder { get; private set; } = null!;
    public User Approver { get; private set; } = null!;

    // Constructor for EF Core
    private ChangeOrderApproval() { }

    public ChangeOrderApproval(
        Guid changeOrderId,
        string approverId,
        string approvalLevel,
        decimal authorityLimit)
    {
        ChangeOrderId = changeOrderId;
        ApproverId = approverId ?? throw new ArgumentNullException(nameof(approverId));
        ApprovalLevel = approvalLevel ?? throw new ArgumentNullException(nameof(approvalLevel));
        AuthorityLimit = authorityLimit;

        DecisionDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }

    public void MakeDecision(
        ApprovalDecision decision,
        bool isWithinAuthority,
        string? comments = null,
        string? conditions = null)
    {
        Decision = decision;
        IsWithinAuthority = isWithinAuthority;
        Comments = comments;

        if (!string.IsNullOrWhiteSpace(conditions))
        {
            Conditions = conditions;
            IsConditional = true;
        }

        DecisionDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}