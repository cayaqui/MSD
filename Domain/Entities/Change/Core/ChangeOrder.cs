using Domain.Common;
using Domain.Entities.Auth.Security;
using Domain.Entities.Cost.Commitments;
using Domain.Entities.Cost.Control;
using Domain.Entities.Organization.Core;
using Domain.Entities.Risks.Core;
using Core.Enums.Change;
using Core.Enums.Cost;
using Domain.Entities.WBS;

namespace Domain.Entities.Change.Core;

/// <summary>
/// Change Order entity following PMI Change Management standards
/// Format: CO-XXX-###
/// </summary>
public class ChangeOrder : BaseEntity, IAuditable, ISoftDelete
{
    // Basic Information
    public string ChangeOrderNumber { get; private set; } = string.Empty; // CO-XXX-###
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Justification { get; private set; } = string.Empty;

    // Classification
    public ChangeOrderType Type { get; private set; }
    public ChangeOrderCategory Category { get; private set; }
    public ChangeOrderPriority Priority { get; private set; }
    public ChangeOrderSource Source { get; private set; }

    // Impact Assessment
    public decimal EstimatedCostImpact { get; private set; }
    public int ScheduleImpactDays { get; private set; }
    public string? ScopeImpact { get; private set; }
    public string? QualityImpact { get; private set; }
    public string? RiskImpact { get; private set; }

    // Financial Details
    public decimal? ApprovedAmount { get; private set; }
    public string Currency { get; private set; } = "USD";
    public decimal? ContingencyUsed { get; private set; }

    // Status and Workflow
    public ChangeOrderStatus Status { get; private set; }
    public string? CurrentApprovalLevel { get; private set; }
    public int ApprovalSequence { get; private set; }

    // Relationships
    public Guid ProjectId { get; private set; }
    public Guid? ControlAccountId { get; private set; }
    public Guid? CommitmentId { get; private set; }
    public Guid? WBSElementId { get; private set; }

    // Request Information
    public string RequestedById { get; private set; } = string.Empty;
    public DateTime RequestDate { get; private set; }
    public string? RequestorDepartment { get; private set; }

    // Approval Information
    public string? SubmittedById { get; private set; }
    public DateTime? SubmittedDate { get; private set; }
    public string? ReviewedById { get; private set; }
    public DateTime? ReviewedDate { get; private set; }
    public string? ApprovedById { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? RejectedById { get; private set; }
    public DateTime? RejectionDate { get; private set; }
    public string? RejectionReason { get; private set; }

    // Implementation
    public DateTime? ImplementationStartDate { get; private set; }
    public DateTime? ImplementationEndDate { get; private set; }
    public string? ImplementationNotes { get; private set; }

    // Supporting Documentation
    public string? AttachmentsJson { get; private set; } // JSON array of document references
    public string? ImpactAnalysisDocument { get; private set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public Project Project { get; private set; } = null!;
    public ControlAccount? ControlAccount { get; private set; }
    public Commitment? Commitment { get; private set; }
    public WBSElement? WBSElement { get; private set; }
    public User RequestedBy { get; private set; } = null!;
    public User? SubmittedBy { get; private set; }
    public User? ReviewedBy { get; private set; }
    public User? ApprovedBy { get; private set; }
    public User? RejectedBy { get; private set; }
    public ICollection<ChangeOrderApproval> Approvals { get; private set; } =
        new List<ChangeOrderApproval>();
    public ICollection<ChangeOrderImpact> Impacts { get; private set; } =
        new List<ChangeOrderImpact>();

    // Constructor for EF Core
    private ChangeOrder() { }

    public ChangeOrder(
        string changeOrderNumber,
        string title,
        string description,
        string justification,
        Guid projectId,
        string requestedById,
        ChangeOrderType type,
        ChangeOrderCategory category,
        ChangeOrderSource source
    )
    {
        ChangeOrderNumber =
            changeOrderNumber ?? throw new ArgumentNullException(nameof(changeOrderNumber));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Justification = justification ?? throw new ArgumentNullException(nameof(justification));
        ProjectId = projectId;
        RequestedById = requestedById ?? throw new ArgumentNullException(nameof(requestedById));
        Type = type;
        Category = category;
        Source = source;

        Status = ChangeOrderStatus.Draft;
        Priority = ChangeOrderPriority.Medium;
        RequestDate = DateTime.UtcNow;
        ApprovalSequence = 0;

        ValidateChangeOrderNumber();
    }

    // Domain Methods
    public void AssessImpact(
        decimal costImpact,
        int scheduleImpactDays,
        string? scopeImpact = null,
        string? qualityImpact = null,
        string? riskImpact = null
    )
    {
        if (Status != ChangeOrderStatus.Draft && Status != ChangeOrderStatus.UnderReview)
            throw new InvalidOperationException(
                "Can only assess impact for draft or under review change orders"
            );

        EstimatedCostImpact = costImpact;
        ScheduleImpactDays = scheduleImpactDays;
        ScopeImpact = scopeImpact;
        QualityImpact = qualityImpact;
        RiskImpact = riskImpact;

        // Auto-set priority based on impact
        Priority = DeterminePriority(costImpact, scheduleImpactDays);

        UpdatedAt = DateTime.UtcNow;
    }

    public void Submit(string submittedById)
    {
        if (Status != ChangeOrderStatus.Draft)
            throw new InvalidOperationException("Can only submit draft change orders");

        if (EstimatedCostImpact == 0 && ScheduleImpactDays == 0)
            throw new InvalidOperationException("Must assess impact before submission");

        SubmittedById = submittedById ?? throw new ArgumentNullException(nameof(submittedById));
        SubmittedDate = DateTime.UtcNow;
        Status = ChangeOrderStatus.Submitted;

        UpdatedAt = DateTime.UtcNow;
    }

    public void StartReview(string reviewerId)
    {
        if (Status != ChangeOrderStatus.Submitted)
            throw new InvalidOperationException("Can only review submitted change orders");

        ReviewedById = reviewerId ?? throw new ArgumentNullException(nameof(reviewerId));
        ReviewedDate = DateTime.UtcNow;
        Status = ChangeOrderStatus.UnderReview;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string approvedById, decimal? approvedAmount = null)
    {
        if (Status != ChangeOrderStatus.UnderReview && Status != ChangeOrderStatus.Submitted)
            throw new InvalidOperationException("Can only approve change orders under review");

        ApprovedById = approvedById ?? throw new ArgumentNullException(nameof(approvedById));
        ApprovalDate = DateTime.UtcNow;
        ApprovedAmount = approvedAmount ?? EstimatedCostImpact;
        Status = ChangeOrderStatus.Approved;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string rejectedById, string reason)
    {
        if (Status == ChangeOrderStatus.Approved || Status == ChangeOrderStatus.Implemented)
            throw new InvalidOperationException(
                "Cannot reject approved or implemented change orders"
            );

        RejectedById = rejectedById ?? throw new ArgumentNullException(nameof(rejectedById));
        RejectionDate = DateTime.UtcNow;
        RejectionReason = reason ?? throw new ArgumentNullException(nameof(reason));
        Status = ChangeOrderStatus.Rejected;

        UpdatedAt = DateTime.UtcNow;
    }

    public void StartImplementation(DateTime startDate, DateTime plannedEndDate)
    {
        if (Status != ChangeOrderStatus.Approved)
            throw new InvalidOperationException("Can only implement approved change orders");

        if (plannedEndDate <= startDate)
            throw new ArgumentException("End date must be after start date");

        ImplementationStartDate = startDate;
        ImplementationEndDate = plannedEndDate;
        Status = ChangeOrderStatus.InProgress;

        UpdatedAt = DateTime.UtcNow;
    }

    public void CompleteImplementation(string? notes = null)
    {
        if (Status != ChangeOrderStatus.InProgress)
            throw new InvalidOperationException("Can only complete in-progress change orders");

        ImplementationEndDate = DateTime.UtcNow;
        ImplementationNotes = notes;
        Status = ChangeOrderStatus.Implemented;

        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkToControlAccount(Guid controlAccountId)
    {
        ControlAccountId = controlAccountId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkToCommitment(Guid commitmentId)
    {
        CommitmentId = commitmentId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkToWBS(Guid wbsElementId)
    {
        WBSElementId = wbsElementId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateContingencyUsed(decimal amount)
    {
        if (Status != ChangeOrderStatus.Approved && Status != ChangeOrderStatus.InProgress)
            throw new InvalidOperationException(
                "Can only update contingency for approved change orders"
            );

        ContingencyUsed = amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddApprovalRecord(ChangeOrderApproval approval)
    {
        Approvals.Add(approval);
        ApprovalSequence++;
        CurrentApprovalLevel = approval.ApprovalLevel;
        UpdatedAt = DateTime.UtcNow;
    }

    // Private Methods
    private void ValidateChangeOrderNumber()
    {
        // Format: CO-XXX-###
        if (!System.Text.RegularExpressions.Regex.IsMatch(ChangeOrderNumber, @"^CO-\d{3}-\d{3}$"))
            throw new ArgumentException(
                "Change order number must follow format CO-XXX-### (e.g., CO-001-001)"
            );
    }

    private ChangeOrderPriority DeterminePriority(decimal costImpact, int scheduleImpactDays)
    {
        // Priority matrix based on cost and schedule impact
        var costScore = costImpact switch
        {
            > 1000000 => 5,
            > 500000 => 4,
            > 100000 => 3,
            > 50000 => 2,
            _ => 1,
        };

        var scheduleScore = scheduleImpactDays switch
        {
            > 30 => 5,
            > 14 => 4,
            > 7 => 3,
            > 3 => 2,
            _ => 1,
        };

        var totalScore = Math.Max(costScore, scheduleScore);

        return totalScore switch
        {
            5 => ChangeOrderPriority.Critical,
            4 => ChangeOrderPriority.Urgent,
            3 => ChangeOrderPriority.High,
            2 => ChangeOrderPriority.Medium,
            _ => ChangeOrderPriority.Low
        };
    }
}
