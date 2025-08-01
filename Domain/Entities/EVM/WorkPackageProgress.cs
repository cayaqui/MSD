using Core.Enums.Progress;

namespace Domain.Entities.EVM;

/// <summary>
/// Work Package Progress record for tracking progress history
/// </summary>
public class WorkPackageProgress : BaseEntity, IAuditable
{
    // Basic Information
    public Guid WorkPackageId { get; private set; }
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

    // Constructor for EF Core
    private WorkPackageProgress() { }

    public WorkPackageProgress(
        Guid workPackageId,
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
        WorkPackageId = workPackageId;
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

    // Methods
    public void UpdateProgress(decimal newProgress, string? comments = null)
    {
        PreviousProgress = CurrentProgress;
        CurrentProgress = newProgress;
        if (!string.IsNullOrEmpty(comments))
        {
            Comments = comments;
        }
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void UpdateCosts(decimal actualCost, decimal committedCost, decimal forecastCost)
    {
        PreviousActualCost = CurrentActualCost;
        CurrentActualCost = actualCost;
        CommittedCost = committedCost;
        ForecastCost = forecastCost;
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void SetEarnedValueMetrics(decimal earnedValue, decimal plannedValue)
    {
        EarnedValue = earnedValue;
        PlannedValue = plannedValue;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddIssue(string issue)
    {
        Issues = string.IsNullOrEmpty(Issues) ? issue : $"{Issues}; {issue}";
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddRisk(string risk)
    {
        Risks = string.IsNullOrEmpty(Risks) ? risk : $"{Risks}; {risk}";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string userId)
    {
        IsApproved = true;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string userId, string reason)
    {
        IsApproved = false;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = userId;
        Comments = $"REJECTED: {reason}. {Comments}";
        UpdatedAt = DateTime.UtcNow;
    }

    public void AttachPhotos(string[] photoIds)
    {
        PhotoReferences = System.Text.Json.JsonSerializer.Serialize(photoIds);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AttachDocuments(string[] documentIds)
    {
        DocumentReferences = System.Text.Json.JsonSerializer.Serialize(documentIds);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetScheduleImpact(DateTime? actualStartDate, DateTime? actualEndDate, int? daysDelayed)
    {
        ActualStartDate = actualStartDate;
        ActualEndDate = actualEndDate;
        DaysDelayed = daysDelayed;
        UpdatedAt = DateTime.UtcNow;
    }

    // Private Methods
    private int CalculateProgressPeriod(DateTime date)
    {
        // For weekly progress, return week number
        // For monthly progress, return month number
        return System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
            date,
            System.Globalization.CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek.Monday);
    }

    private void Validate()
    {
        if (CurrentProgress < 0 || CurrentProgress > 100)
            throw new ArgumentException("Progress must be between 0 and 100");

        if (CurrentProgress < PreviousProgress)
            throw new ArgumentException("Current progress cannot be less than previous progress");

        if (CurrentActualCost < 0 || CommittedCost < 0 || ForecastCost < 0)
            throw new ArgumentException("Cost values cannot be negative");
    }
}
