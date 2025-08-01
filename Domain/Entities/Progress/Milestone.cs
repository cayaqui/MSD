using Core.Enums.Progress;
using Domain.Entities.Projects;

namespace Domain.Entities.Progress;

/// <summary>
/// Milestone entity for tracking key project milestones
/// </summary>
public class Milestone : BaseEntity, IAuditable, ISoftDelete
{
    // Basic Information
    public string MilestoneCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Classification
    public Guid ProjectId { get; private set; }
    public Guid? PhaseId { get; private set; }
    public Guid? WorkPackageId { get; private set; }
    public MilestoneType Type { get; private set; }
    public bool IsCritical { get; private set; }
    public bool IsContractual { get; private set; }

    // Schedule Information
    public DateTime PlannedDate { get; private set; }
    public DateTime? ForecastDate { get; private set; }
    public DateTime? ActualDate { get; private set; }
    public int? DaysVariance => ActualDate.HasValue
        ? (ActualDate.Value - PlannedDate).Days
        : ForecastDate.HasValue
            ? (ForecastDate.Value - PlannedDate).Days
            : null;

    // Status
    public bool IsCompleted { get; private set; }
    public decimal CompletionPercentage { get; private set; }
    public string? CompletionCriteria { get; private set; }

    // Financial Impact
    public decimal? PaymentAmount { get; private set; }
    public string? PaymentCurrency { get; private set; }
    public bool IsPaymentTriggered { get; private set; }

    // Dependencies
    public string? PredecessorMilestones { get; private set; } // JSON array
    public string? SuccessorMilestones { get; private set; } // JSON array

    // Approval
    public bool RequiresApproval { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApprovedBy { get; private set; }

    // Documentation
    public string? Deliverables { get; private set; } // JSON array of deliverable IDs
    public string? AcceptanceCriteria { get; private set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public Project Project { get; private set; } = null!;
    public Phase? Phase { get; private set; }

    // Constructor for EF Core
    private Milestone() { }

    public Milestone(
        string milestoneCode,
        string name,
        Guid projectId,
        MilestoneType type,
        DateTime plannedDate,
        bool isCritical = false,
        bool isContractual = false)
    {
        MilestoneCode = milestoneCode ?? throw new ArgumentNullException(nameof(milestoneCode));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ProjectId = projectId;
        Type = type;
        PlannedDate = plannedDate;
        IsCritical = isCritical;
        IsContractual = isContractual;

        IsCompleted = false;
        CompletionPercentage = 0;
        RequiresApproval = isContractual; // Contractual milestones require approval
        IsApproved = false;
        IsPaymentTriggered = false;
        CreatedAt = DateTime.UtcNow;
    }

    // Methods
    public void UpdateDetails(string name, string? description, bool isCritical)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        IsCritical = isCritical;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSchedule(DateTime? forecastDate)
    {
        ForecastDate = forecastDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProgress(decimal completionPercentage)
    {
        if (completionPercentage < 0 || completionPercentage > 100)
            throw new ArgumentException("Completion percentage must be between 0 and 100");

        CompletionPercentage = completionPercentage;

        if (completionPercentage == 100 && !IsCompleted)
        {
            Complete();
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(DateTime? actualDate = null)
    {
        IsCompleted = true;
        CompletionPercentage = 100;
        ActualDate = actualDate ?? DateTime.UtcNow;

        if (IsContractual && PaymentAmount.HasValue)
        {
            IsPaymentTriggered = true;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string userId)
    {
        if (!RequiresApproval)
            throw new InvalidOperationException("This milestone does not require approval");

        IsApproved = true;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPaymentTerms(decimal amount, string currency)
    {
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be positive");

        PaymentAmount = amount;
        PaymentCurrency = currency ?? throw new ArgumentNullException(nameof(currency));
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCompletionCriteria(string criteria)
    {
        CompletionCriteria = criteria;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAcceptanceCriteria(string criteria)
    {
        AcceptanceCriteria = criteria;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToPhase(Guid phaseId)
    {
        PhaseId = phaseId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToWorkPackage(Guid workPackageId)
    {
        WorkPackageId = workPackageId;
        UpdatedAt = DateTime.UtcNow;
    }
}
