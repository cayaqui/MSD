using Core.Enums.Progress;
using Domain.Entities.Projects;

namespace Domain.Entities.Progress;

/// <summary>
/// WBS Element Progress record for tracking progress history at any WBS level
/// </summary>
public class WBSElementProgress : BaseEntity, IAuditable
{
    // Basic Information
    public Guid WBSElementId { get; private set; }
    public DateTime ProgressDate { get; private set; }
    public int ProgressPeriod { get; private set; } // Week or Month number
    public int Year { get; private set; }

    // Progress Information
    public decimal PreviousProgress { get; private set; }
    public decimal CurrentProgress { get; private set; }
    public decimal ProgressDelta => CurrentProgress - PreviousProgress;
    public ProgressMethod MeasurementMethod { get; private set; }

    // Cost Information
    public decimal PreviousActualCost { get; private set; }
    public decimal CurrentActualCost { get; private set; }
    public decimal CostDelta => CurrentActualCost - PreviousActualCost;
    public decimal CommittedCost { get; private set; }
    public decimal ForecastCost { get; private set; }

    // Schedule Information
    public DateTime? ActualStartDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }
    public int? DaysDelayed { get; private set; }

    // Earned Value at time of progress
    public decimal? EarnedValue { get; private set; }
    public decimal? PlannedValue { get; private set; }
    public decimal? ScheduleVariance => EarnedValue.HasValue && PlannedValue.HasValue
        ? EarnedValue.Value - PlannedValue.Value
        : null;

    // Details
    public string? Comments { get; private set; }
    public string? Issues { get; private set; }
    public string? Risks { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApprovedBy { get; private set; }

    // Supporting Documentation
    public string? PhotoReferences { get; private set; } // JSON array of photo IDs
    public string? DocumentReferences { get; private set; } // JSON array of document IDs

    // Navigation Properties
    public WBSElement WBSElement { get; private set; } = null!;

    // Constructor for EF Core
    private WBSElementProgress() { }

    public WBSElementProgress(
        Guid wbsElementId,
        DateTime progressDate,
        decimal previousProgress,
        decimal currentProgress,
        decimal previousActualCost,
        decimal currentActualCost,
        decimal committedCost,
        decimal forecastCost,
        ProgressMethod measurementMethod,
        string? comments = null)
    {
        WBSElementId = wbsElementId;
        ProgressDate = progressDate;
        PreviousProgress = previousProgress;
        CurrentProgress = currentProgress;
        PreviousActualCost = previousActualCost;
        CurrentActualCost = currentActualCost;
        CommittedCost = committedCost;
        ForecastCost = forecastCost;
        MeasurementMethod = measurementMethod;
        Comments = comments;

        Year = progressDate.Year;
        ProgressPeriod = CalculateProgressPeriod(progressDate);
        IsApproved = false;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    // Métodos igual que antes...
}