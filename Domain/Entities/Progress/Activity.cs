using System;
using System.Collections.Generic;
using Domain.Common;
using Core.Enums.Progress;
using Domain.Entities.WBS;

namespace Domain.Entities.Progress;

/// <summary>
/// Activity for detailed work breakdown
/// </summary>
public class Activity : BaseEntity, IAuditable
{
    // Basic Information
    public Guid WBSElementId { get; private set; } // Updated from WorkPackageId
    public string ActivityCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Schedule Information
    public DateTime PlannedStartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public DateTime? ActualStartDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }
    public int DurationDays { get; private set; }

    // Progress Information
    public decimal ProgressPercentage { get; private set; }
    public ActivityStatus Status { get; private set; }

    // Resources
    public decimal PlannedHours { get; private set; }
    public decimal ActualHours { get; private set; }
    public decimal? ResourceRate { get; private set; }

    // Dependencies
    public string? PredecessorActivities { get; private set; } // JSON array of activity codes
    public string? SuccessorActivities { get; private set; } // JSON array of activity codes

    // Navigation Properties
    public WBSElement WBSElement { get; private set; } = null!; // Updated
    public WorkPackageDetails? WorkPackageDetails { get; private set; } // Through WBSElement
    public ICollection<Resource> Resources { get; private set; } = new List<Resource>();

    // Constructor for EF Core
    private Activity() { }

    public Activity(
        Guid wbsElementId,
        string activityCode,
        string name,
        DateTime plannedStartDate,
        DateTime plannedEndDate,
        decimal plannedHours)
    {
        WBSElementId = wbsElementId;
        ActivityCode = activityCode ?? throw new ArgumentNullException(nameof(activityCode));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        PlannedHours = plannedHours;

        DurationDays = (plannedEndDate - plannedStartDate).Days;
        Status = ActivityStatus.NotStarted;
        ProgressPercentage = 0;
        ActualHours = 0;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    // Methods
    public void Start()
    {
        if (Status != ActivityStatus.NotStarted)
            throw new InvalidOperationException("Activity has already started");

        Status = ActivityStatus.InProgress;
        ActualStartDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProgress(decimal percentage, decimal actualHours)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Progress must be between 0 and 100");

        if (actualHours < 0)
            throw new ArgumentException("Actual hours cannot be negative");

        ProgressPercentage = percentage;
        ActualHours = actualHours;

        if (percentage == 100 && Status != ActivityStatus.Completed)
        {
            Complete();
        }
        else if (percentage > 0 && Status == ActivityStatus.NotStarted)
        {
            Start();
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = ActivityStatus.Completed;
        ActualEndDate = DateTime.UtcNow;
        ProgressPercentage = 100;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Suspend()
    {
        if (Status != ActivityStatus.InProgress)
            throw new InvalidOperationException("Only in-progress activities can be suspended");

        Status = ActivityStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Resume()
    {
        if (Status != ActivityStatus.Suspended)
            throw new InvalidOperationException("Only suspended activities can be resumed");

        Status = ActivityStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == ActivityStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed activities");

        Status = ActivityStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSchedule(DateTime plannedStartDate, DateTime plannedEndDate)
    {
        if (plannedEndDate <= plannedStartDate)
            throw new ArgumentException("End date must be after start date");

        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        DurationDays = (plannedEndDate - plannedStartDate).Days;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetResourceRate(decimal rate)
    {
        if (rate < 0)
            throw new ArgumentException("Resource rate cannot be negative");

        ResourceRate = rate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDependencies(string[]? predecessors, string[]? successors)
    {
        PredecessorActivities = predecessors != null ? System.Text.Json.JsonSerializer.Serialize(predecessors) : null;
        SuccessorActivities = successors != null ? System.Text.Json.JsonSerializer.Serialize(successors) : null;
        UpdatedAt = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (PlannedEndDate < PlannedStartDate)
            throw new ArgumentException("End date must be after start date");

        if (PlannedHours < 0 || ActualHours < 0)
            throw new ArgumentException("Hours cannot be negative");
    }

    // Helper methods
    public bool IsOnSchedule()
    {
        if (!ActualStartDate.HasValue)
            return true;

        return ActualStartDate.Value.Date <= PlannedStartDate.Date;
    }

    public int GetDelayDays()
    {
        if (!ActualEndDate.HasValue || ActualEndDate.Value <= PlannedEndDate)
            return 0;

        return (ActualEndDate.Value - PlannedEndDate).Days;
    }

    public decimal GetProductivityRate()
    {
        if (ActualHours == 0 || ProgressPercentage == 0)
            return 0;

        var expectedHours = PlannedHours * (ProgressPercentage / 100);
        return expectedHours / ActualHours;
    }
}