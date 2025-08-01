using Domain.Common;
using Domain.Entities.Cost;
using System;

namespace Domain.Entities.Projects;

/// <summary>
/// Planning Package entity
/// Represents future work that is not yet defined in detail
/// Will be converted to Work Packages as planning progresses
/// </summary>
public class PlanningPackage : BaseEntity, IAuditable, ISoftDelete, IActivatable
{
    // Basic Information
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Foreign Keys
    public Guid ControlAccountId { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid PhaseId { get; private set; }

    // Planning Information
    public DateTime PlannedStartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public decimal EstimatedBudget { get; private set; }
    public decimal EstimatedHours { get; private set; }

    // Conversion Information
    public DateTime PlannedConversionDate { get; private set; } // When to convert to Work Packages
    public bool IsConverted { get; private set; }
    public DateTime? ConversionDate { get; private set; }
    public string? ConvertedBy { get; private set; }

    // Status
    public PlanningPackageStatus Status { get; private set; }
    public int Priority { get; private set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Activatable
    public bool IsActive { get; set; }

    // Navigation Properties
    public ControlAccount ControlAccount { get; private set; } = null!;
    public Project Project { get; private set; } = null!;
    public Phase Phase { get; private set; } = null!;

    // Constructor for EF Core
    private PlanningPackage() { }

    public PlanningPackage(
        string code,
        string name,
        Guid controlAccountId,
        Guid projectId,
        Guid phaseId,
        DateTime plannedStartDate,
        DateTime plannedEndDate,
        decimal estimatedBudget)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ControlAccountId = controlAccountId;
        ProjectId = projectId;
        PhaseId = phaseId;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        EstimatedBudget = estimatedBudget;
        PlannedConversionDate = plannedStartDate.AddDays(-30); // Default: 30 days before start
        Status = PlanningPackageStatus.Future;
        Priority = 99; // Low priority by default
        IsActive = true;
    }

    // Domain Methods
    public void UpdateDetails(
        string name,
        string? description,
        decimal estimatedBudget,
        decimal estimatedHours,
        string updatedBy)
    {
        if (IsConverted)
            throw new InvalidOperationException("Cannot update a converted Planning Package");

        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        EstimatedBudget = estimatedBudget;
        EstimatedHours = estimatedHours;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSchedule(
        DateTime plannedStartDate,
        DateTime plannedEndDate,
        DateTime plannedConversionDate,
        string updatedBy)
    {
        if (IsConverted)
            throw new InvalidOperationException("Cannot update schedule of a converted Planning Package");

        if (plannedEndDate < plannedStartDate)
            throw new ArgumentException("End date cannot be before start date");

        if (plannedConversionDate > plannedStartDate)
            throw new ArgumentException("Conversion date should be before the planned start date");

        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        PlannedConversionDate = plannedConversionDate;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;

        UpdateStatus();
    }

    public void UpdatePriority(int priority, string updatedBy)
    {
        if (priority < 1 || priority > 99)
            throw new ArgumentException("Priority must be between 1 (highest) and 99 (lowest)");

        Priority = priority;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConvertToWorkPackage(string convertedBy)
    {
        if (IsConverted)
            throw new InvalidOperationException("Planning Package has already been converted");

        IsConverted = true;
        ConversionDate = DateTime.UtcNow;
        ConvertedBy = convertedBy;
        Status = PlanningPackageStatus.Converted;
        IsActive = false; // Deactivate after conversion
        UpdatedBy = convertedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus()
    {
        if (IsConverted)
        {
            Status = PlanningPackageStatus.Converted;
        }
        else if (DateTime.UtcNow >= PlannedConversionDate)
        {
            Status = PlanningPackageStatus.ReadyForConversion;
        }
        else if ((PlannedConversionDate - DateTime.UtcNow).TotalDays <= 60)
        {
            Status = PlanningPackageStatus.NearTerm;
        }
        else
        {
            Status = PlanningPackageStatus.Future;
        }
    }

    public bool IsReadyForConversion()
    {
        return !IsConverted &&
               Status == PlanningPackageStatus.ReadyForConversion &&
               IsActive;
    }

    public int GetDaysUntilConversion()
    {
        if (IsConverted) return 0;
        var days = (PlannedConversionDate - DateTime.UtcNow).TotalDays;
        return days > 0 ? (int)days : 0;
    }
}