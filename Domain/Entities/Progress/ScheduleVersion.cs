using System;
using Domain.Common;
using Core.Enums.Progress;
using Domain.Entities.Projects;

namespace Domain.Entities.Progress;

/// <summary>
/// Schedule Version for tracking project schedule baselines and updates
/// </summary>
public class ScheduleVersion : BaseEntity, IAuditable
{
    // Basic Information
    public Guid ProjectId { get; private set; }
    public string Version { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Status
    public ScheduleStatus Status { get; private set; }
    public bool IsBaseline { get; private set; }
    public DateTime? BaselineDate { get; private set; }

    // Schedule Dates
    public DateTime PlannedStartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public int TotalDuration => (PlannedEndDate - PlannedStartDate).Days;

    // Statistics
    public int TotalActivities { get; private set; }
    public int CriticalActivities { get; private set; }
    public decimal TotalFloat { get; private set; }
    public DateTime? DataDate { get; private set; }

    // Approval
    public DateTime? SubmittedDate { get; private set; }
    public string? SubmittedBy { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApprovedBy { get; private set; }
    public string? ApprovalComments { get; private set; }

    // Import Information
    public string? SourceSystem { get; private set; } // MS Project, Primavera, etc.
    public DateTime? ImportDate { get; private set; }
    public string? ImportedBy { get; private set; }

    // Navigation Properties
    public Project Project { get; private set; } = null!;
    public ICollection<Activity> Activities { get; private set; } = new List<Activity>();

    // Constructor for EF Core
    private ScheduleVersion() { }

    public ScheduleVersion(
        Guid projectId,
        string version,
        string name,
        DateTime plannedStartDate,
        DateTime plannedEndDate)
    {
        ProjectId = projectId;
        Version = version ?? throw new ArgumentNullException(nameof(version));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;

        Status = ScheduleStatus.Draft;
        IsBaseline = false;
        TotalActivities = 0;
        CriticalActivities = 0;
        TotalFloat = 0;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    // Methods
    public void UpdateStatistics(int totalActivities, int criticalActivities, decimal totalFloat)
    {
        TotalActivities = totalActivities;
        CriticalActivities = criticalActivities;
        TotalFloat = totalFloat;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDataDate(DateTime dataDate)
    {
        DataDate = dataDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Submit(string userId)
    {
        if (Status != ScheduleStatus.Draft)
            throw new InvalidOperationException("Only draft schedules can be submitted");

        Status = ScheduleStatus.UnderReview;
        SubmittedDate = DateTime.UtcNow;
        SubmittedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string userId, string? comments = null)
    {
        if (Status != ScheduleStatus.UnderReview)
            throw new InvalidOperationException("Only schedules under review can be approved");

        Status = ScheduleStatus.Approved;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = userId;
        ApprovalComments = comments;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAsBaseline()
    {
        if (Status != ScheduleStatus.Approved)
            throw new InvalidOperationException("Only approved schedules can be set as baseline");

        Status = ScheduleStatus.Baselined;
        IsBaseline = true;
        BaselineDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordImport(string sourceSystem, string userId)
    {
        SourceSystem = sourceSystem;
        ImportDate = DateTime.UtcNow;
        ImportedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (PlannedEndDate < PlannedStartDate)
            throw new ArgumentException("End date must be after start date");
    }
}