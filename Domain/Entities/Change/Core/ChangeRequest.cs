using Domain.Common;
using Domain.Entities.Auth.Security;
using Domain.Entities.Contracts.Core;
using Domain.Entities.Cost.Control;
using Domain.Entities.Organization.Core;
using Core.Enums.Change;
using Core.Enums.Cost;
using Domain.Entities.WBS;

namespace Domain.Entities.Change.Core;

/// <summary>
/// Change Request entity for formal change management process
/// Represents the initial formal request for project changes before becoming a Change Order
/// </summary>
public class ChangeRequest : BaseEntity, IAuditable, ISoftDelete, ICodeEntity
{
    // Basic Information
    public string Code { get; set; } = string.Empty; // CR-XXX-YYYY format
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public ChangeRequestPriority Priority { get; private set; }

    // Classification
    public Guid ProjectId { get; private set; }
    public Guid? TrendId { get; private set; }
    public Guid RequestorId { get; private set; }
    public ChangeRequestType Type { get; private set; }
    public ChangeRequestCategory Category { get; private set; }
    public ChangeRequestStatus Status { get; private set; }
    public ChangeRequestSource Source { get; private set; }

    // Affected Areas
    public Guid? WBSElementId { get; private set; }
    public Guid? ControlAccountId { get; private set; }
    public Guid? ContractorId { get; private set; }
    public string? AffectedPackages { get; private set; } // JSON array of package IDs
    public string? AffectedDisciplines { get; private set; } // JSON array of discipline IDs

    // Dates
    public DateTime RequestDate { get; private set; }
    public DateTime RequiredByDate { get; private set; }
    public DateTime? ReviewStartDate { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public DateTime? RejectionDate { get; private set; }
    public DateTime? ImplementationStartDate { get; private set; }
    public DateTime? ImplementationEndDate { get; private set; }

    // Impact Assessment Summary
    public bool HasCostImpact { get; private set; }
    public bool HasScheduleImpact { get; private set; }
    public bool HasQualityImpact { get; private set; }
    public bool HasScopeImpact { get; private set; }
    public bool HasSafetyImpact { get; private set; }
    public ChangeRequestImpactLevel OverallImpactLevel { get; private set; }

    // Cost Impact Details
    public decimal? EstimatedCostImpact { get; private set; }
    public decimal? ApprovedCostImpact { get; private set; }
    public string Currency { get; private set; } = "USD";
    public CostImpactType? CostImpactType { get; private set; }
    public string? CostBreakdown { get; private set; } // JSON structure

    // Schedule Impact Details
    public int? EstimatedScheduleImpactDays { get; private set; }
    public int? ApprovedScheduleImpactDays { get; private set; }
    public DateTime? CurrentCompletionDate { get; private set; }
    public DateTime? ProposedCompletionDate { get; private set; }
    public bool AffectsCriticalPath { get; private set; }
    public string? AffectedMilestones { get; private set; } // JSON array

    // Technical Details
    public string? TechnicalJustification { get; private set; }
    public string? ProposedSolution { get; private set; }
    public string? AlternativeOptions { get; private set; }
    public string? RiskAssessment { get; private set; }
    public string? QualityImpactDescription { get; private set; }
    public string? SafetyImpactDescription { get; private set; }

    // Approval Workflow
    public string? ApprovalRoute { get; private set; } // JSON array of approval steps
    public int CurrentApprovalLevel { get; private set; }
    public int RequiredApprovalLevel { get; private set; }
    public bool RequiresTechnicalReview { get; private set; }
    public bool RequiresClientApproval { get; private set; }
    public bool IsUrgent { get; private set; }

    // Review Details
    public Guid? TechnicalReviewerId { get; private set; }
    public DateTime? TechnicalReviewDate { get; private set; }
    public string? TechnicalReviewComments { get; private set; }
    public bool? TechnicallyApproved { get; private set; }

    public Guid? CostReviewerId { get; private set; }
    public DateTime? CostReviewDate { get; private set; }
    public string? CostReviewComments { get; private set; }
    public bool? CostApproved { get; private set; }

    public Guid? ScheduleReviewerId { get; private set; }
    public DateTime? ScheduleReviewDate { get; private set; }
    public string? ScheduleReviewComments { get; private set; }
    public bool? ScheduleApproved { get; private set; }

    // Final Approval
    public Guid? ApprovedById { get; private set; }
    public string? ApprovalComments { get; private set; }
    public string? ApprovalConditions { get; private set; }
    public string? RejectionReason { get; private set; }

    // Client Details
    public string? ClientReferenceNumber { get; private set; }
    public bool? ClientApproved { get; private set; }
    public DateTime? ClientApprovalDate { get; private set; }
    public string? ClientComments { get; private set; }

    // Implementation Tracking
    public bool IsImplemented { get; private set; }
    public decimal? ActualCostImpact { get; private set; }
    public int? ActualScheduleImpactDays { get; private set; }
    public string? ImplementationNotes { get; private set; }
    public string? LessonsLearned { get; private set; }

    // Change Order Reference
    public Guid? ChangeOrderId { get; private set; }
    public bool IsConvertedToChangeOrder { get; private set; }

    // Supporting Documentation
    public string? DrawingReferences { get; private set; } // JSON array
    public string? SpecificationReferences { get; private set; } // JSON array
    public string? StandardReferences { get; private set; } // JSON array

    // Calculated Fields
    public int DaysInReview =>
        ReviewStartDate.HasValue ? (DateTime.UtcNow - ReviewStartDate.Value).Days : 0;

    public bool IsOverdue =>
        Status != ChangeRequestStatus.Closed
        && Status != ChangeRequestStatus.Cancelled
        && DateTime.UtcNow > RequiredByDate;

    public decimal? CostVariance =>
        ActualCostImpact.HasValue && ApprovedCostImpact.HasValue
            ? ActualCostImpact.Value - ApprovedCostImpact.Value
            : null;

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public Project Project { get; private set; } = null!;
    public Trend? Trend { get; private set; }
    public User Requestor { get; private set; } = null!;
    public WBSElement? WBSElement { get; private set; }
    public ControlAccount? ControlAccount { get; private set; }
    public Contractor? Contractor { get; private set; }
    public User? TechnicalReviewer { get; private set; }
    public User? CostReviewer { get; private set; }
    public User? ScheduleReviewer { get; private set; }
    public User? ApprovedBy { get; private set; }
    public ChangeOrder? ChangeOrder { get; private set; }
    public ICollection<ChangeRequestAttachment> Attachments { get; private set; } =
        new List<ChangeRequestAttachment>();
    public ICollection<ChangeRequestComment> Comments { get; private set; } =
        new List<ChangeRequestComment>();
    public ICollection<ChangeRequestApproval> Approvals { get; private set; } =
        new List<ChangeRequestApproval>();

    // Constructor for EF Core
    private ChangeRequest() { }

    public ChangeRequest(
        string code,
        string title,
        string description,
        Guid projectId,
        Guid requestorId,
        ChangeRequestType type,
        ChangeRequestCategory category,
        ChangeRequestSource source,
        DateTime requiredByDate
    )
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        ProjectId = projectId;
        RequestorId = requestorId;
        Type = type;
        Category = category;
        Source = source;
        RequiredByDate = requiredByDate;

        RequestDate = DateTime.UtcNow;
        Status = ChangeRequestStatus.Draft;
        Priority = ChangeRequestPriority.Medium;
        OverallImpactLevel = ChangeRequestImpactLevel.Medium;
        CurrentApprovalLevel = 0;
        RequiredApprovalLevel = 1;
        IsImplemented = false;
        IsConvertedToChangeOrder = false;

        CreatedAt = DateTime.UtcNow;
    }

    // Domain Methods
    public void LinkToTrend(Guid trendId)
    {
        if (TrendId.HasValue)
            throw new InvalidOperationException("Change request is already linked to a trend");

        TrendId = trendId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetImpactFlags(
        bool costImpact,
        bool scheduleImpact,
        bool qualityImpact,
        bool scopeImpact,
        bool safetyImpact
    )
    {
        HasCostImpact = costImpact;
        HasScheduleImpact = scheduleImpact;
        HasQualityImpact = qualityImpact;
        HasScopeImpact = scopeImpact;
        HasSafetyImpact = safetyImpact;

        // Auto-calculate overall impact level
        var impactCount = new[]
        {
            costImpact,
            scheduleImpact,
            qualityImpact,
            scopeImpact,
            safetyImpact,
        }.Count(x => x);

        OverallImpactLevel = impactCount switch
        {
            0 => ChangeRequestImpactLevel.None,
            1 => ChangeRequestImpactLevel.Low,
            2 => ChangeRequestImpactLevel.Medium,
            3 => ChangeRequestImpactLevel.High,
            _ => ChangeRequestImpactLevel.Critical,
        };

        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCostImpact(
        decimal estimatedCost,
        CostImpactType impactType,
        string? costBreakdown = null
    )
    {
        HasCostImpact = true;
        EstimatedCostImpact = estimatedCost;
        CostImpactType = impactType;
        CostBreakdown = costBreakdown;

        // Determine approval level based on cost
        RequiredApprovalLevel = Math.Max(
            RequiredApprovalLevel,
            DetermineApprovalLevel(estimatedCost)
        );
        RequiresClientApproval = estimatedCost > 50000; // Example threshold

        UpdatedAt = DateTime.UtcNow;
    }

    public void SetScheduleImpact(
        int impactDays,
        DateTime? proposedCompletion,
        bool criticalPath,
        string? affectedMilestones = null
    )
    {
        HasScheduleImpact = true;
        EstimatedScheduleImpactDays = impactDays;
        ProposedCompletionDate = proposedCompletion;
        AffectsCriticalPath = criticalPath;
        AffectedMilestones = affectedMilestones;

        if (criticalPath || impactDays > 30)
        {
            RequiredApprovalLevel = Math.Max(RequiredApprovalLevel, 3);
            RequiresClientApproval = true;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void SetTechnicalDetails(
        string justification,
        string proposedSolution,
        string? alternatives = null,
        string? riskAssessment = null
    )
    {
        TechnicalJustification =
            justification ?? throw new ArgumentNullException(nameof(justification));
        ProposedSolution =
            proposedSolution ?? throw new ArgumentNullException(nameof(proposedSolution));
        AlternativeOptions = alternatives;
        RiskAssessment = riskAssessment;
        RequiresTechnicalReview = true;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Submit()
    {
        if (Status != ChangeRequestStatus.Draft)
            throw new InvalidOperationException("Only draft change requests can be submitted");

        ValidateReadyForSubmission();

        Status = ChangeRequestStatus.Submitted;
        ReviewStartDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void StartReview()
    {
        if (Status != ChangeRequestStatus.Submitted)
            throw new InvalidOperationException("Only submitted change requests can start review");

        Status = ChangeRequestStatus.UnderReview;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordTechnicalReview(Guid reviewerId, bool approved, string comments)
    {
        TechnicalReviewerId = reviewerId;
        TechnicalReviewDate = DateTime.UtcNow;
        TechnicallyApproved = approved;
        TechnicalReviewComments = comments ?? throw new ArgumentNullException(nameof(comments));

        CheckIfReadyForApproval();
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordCostReview(
        Guid reviewerId,
        bool approved,
        decimal? approvedAmount,
        string comments
    )
    {
        CostReviewerId = reviewerId;
        CostReviewDate = DateTime.UtcNow;
        CostApproved = approved;
        CostReviewComments = comments ?? throw new ArgumentNullException(nameof(comments));

        if (approved && approvedAmount.HasValue)
        {
            ApprovedCostImpact = approvedAmount.Value;
        }

        CheckIfReadyForApproval();
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordScheduleReview(
        Guid reviewerId,
        bool approved,
        int? approvedDays,
        string comments
    )
    {
        ScheduleReviewerId = reviewerId;
        ScheduleReviewDate = DateTime.UtcNow;
        ScheduleApproved = approved;
        ScheduleReviewComments = comments ?? throw new ArgumentNullException(nameof(comments));

        if (approved && approvedDays.HasValue)
        {
            ApprovedScheduleImpactDays = approvedDays.Value;
        }

        CheckIfReadyForApproval();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(Guid approvedById, string? comments = null, string? conditions = null)
    {
        if (Status != ChangeRequestStatus.UnderReview && Status != ChangeRequestStatus.Submitted)
            throw new InvalidOperationException(
                "Change request must be under review to be approved"
            );

        ValidateReviewsComplete();

        Status = ChangeRequestStatus.Approved;
        ApprovalDate = DateTime.UtcNow;
        ApprovedById = approvedById;
        ApprovalComments = comments;
        ApprovalConditions = conditions;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(Guid rejectedById, string reason)
    {
        if (Status == ChangeRequestStatus.Approved || Status == ChangeRequestStatus.Implemented)
            throw new InvalidOperationException(
                "Cannot reject an approved or implemented change request"
            );

        Status = ChangeRequestStatus.Rejected;
        RejectionDate = DateTime.UtcNow;
        ApprovedById = rejectedById;
        RejectionReason = reason ?? throw new ArgumentNullException(nameof(reason));

        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordClientDecision(bool approved, string? referenceNumber, string? comments)
    {
        if (!RequiresClientApproval)
            throw new InvalidOperationException(
                "This change request does not require client approval"
            );

        ClientApproved = approved;
        ClientApprovalDate = DateTime.UtcNow;
        ClientReferenceNumber = referenceNumber;
        ClientComments = comments;

        if (!approved)
        {
            Status = ChangeRequestStatus.ClientRejected;
        }
        else if (Status == ChangeRequestStatus.Approved)
        {
            Status = ChangeRequestStatus.ReadyToImplement;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void ConvertToChangeOrder(Guid changeOrderId)
    {
        if (
            Status != ChangeRequestStatus.Approved
            && Status != ChangeRequestStatus.ReadyToImplement
        )
            throw new InvalidOperationException(
                "Only approved change requests can be converted to change orders"
            );

        ChangeOrderId = changeOrderId;
        IsConvertedToChangeOrder = true;
        Status = ChangeRequestStatus.Converted;

        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsImplemented(
        decimal? actualCost = null,
        int? actualScheduleDays = null,
        string? notes = null
    )
    {
        if (!IsConvertedToChangeOrder)
            throw new InvalidOperationException(
                "Change request must be converted to change order before implementation"
            );

        IsImplemented = true;
        Status = ChangeRequestStatus.Implemented;
        ImplementationEndDate = DateTime.UtcNow;
        ActualCostImpact = actualCost;
        ActualScheduleImpactDays = actualScheduleDays;
        ImplementationNotes = notes;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (Status == ChangeRequestStatus.Implemented || Status == ChangeRequestStatus.Closed)
            throw new InvalidOperationException(
                "Cannot cancel an implemented or closed change request"
            );

        Status = ChangeRequestStatus.Cancelled;
        RejectionReason = reason ?? throw new ArgumentNullException(nameof(reason));

        UpdatedAt = DateTime.UtcNow;
    }

    public void Close(string? lessonsLearned = null)
    {
        if (Status != ChangeRequestStatus.Implemented && Status != ChangeRequestStatus.Rejected)
            throw new InvalidOperationException(
                "Only implemented or rejected change requests can be closed"
            );

        Status = ChangeRequestStatus.Closed;
        LessonsLearned = lessonsLearned;

        UpdatedAt = DateTime.UtcNow;
    }

    public void SetUrgent(bool isUrgent)
    {
        IsUrgent = isUrgent;
        if (isUrgent)
        {
            Priority = ChangeRequestPriority.Urgent;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    // Helper Methods
    private void ValidateReadyForSubmission()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(TechnicalJustification))
            errors.Add("Technical justification is required");

        if (string.IsNullOrWhiteSpace(ProposedSolution))
            errors.Add("Proposed solution is required");

        if (HasCostImpact && !EstimatedCostImpact.HasValue)
            errors.Add("Cost impact amount is required");

        if (HasScheduleImpact && !EstimatedScheduleImpactDays.HasValue)
            errors.Add("Schedule impact days is required");

        if (errors.Any())
            throw new InvalidOperationException(
                $"Change request is not ready for submission: {string.Join(", ", errors)}"
            );
    }

    private void ValidateReviewsComplete()
    {
        var errors = new List<string>();

        if (RequiresTechnicalReview && !TechnicallyApproved.HasValue)
            errors.Add("Technical review is pending");

        if (HasCostImpact && !CostApproved.HasValue)
            errors.Add("Cost review is pending");

        if (HasScheduleImpact && !ScheduleApproved.HasValue)
            errors.Add("Schedule review is pending");

        if (errors.Any())
            throw new InvalidOperationException($"Reviews incomplete: {string.Join(", ", errors)}");
    }

    private void CheckIfReadyForApproval()
    {
        var allReviewsComplete = true;

        if (RequiresTechnicalReview && !TechnicallyApproved.HasValue)
            allReviewsComplete = false;

        if (HasCostImpact && !CostApproved.HasValue)
            allReviewsComplete = false;

        if (HasScheduleImpact && !ScheduleApproved.HasValue)
            allReviewsComplete = false;

        if (allReviewsComplete && Status == ChangeRequestStatus.UnderReview)
        {
            Status = ChangeRequestStatus.PendingApproval;
        }
    }

    private int DetermineApprovalLevel(decimal value)
    {
        return value switch
        {
            <= 10000 => 1, // Project Manager
            <= 50000 => 2, // Operations Manager
            <= 100000 => 3, // Director
            <= 500000 => 4, // VP
            _ => 5, // CEO/Board
        };
    }
}
