using Core.Enums.Cost;
using Core.Enums.Progress;
using Domain.Common;
using Domain.Entities.Security;
using Domain.Entities.Setup;
using System;

namespace Domain.Entities.Projects;

/// <summary>
/// Detalles específicos de un Work Package
/// Solo existe cuando un WBSElement es de tipo WorkPackage
/// </summary>
public class WorkPackageDetails : BaseEntity, IAuditable
{
    // Foreign Key
    public Guid WBSElementId { get; private set; }

    // Schedule Information
    public DateTime PlannedStartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public DateTime? BaselineStartDate { get; private set; }
    public DateTime? BaselineEndDate { get; private set; }
    public DateTime? ActualStartDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }
    public DateTime? ForecastStartDate { get; private set; }
    public DateTime? ForecastEndDate { get; private set; }

    // Duration
    public int PlannedDuration { get; private set; } // in days
    public int? ActualDuration { get; private set; }
    public int? RemainingDuration { get; private set; }
    public decimal TotalFloat { get; private set; }
    public decimal FreeFloat { get; private set; }
    public bool IsCriticalPath { get; private set; }

    // Budget Information
    public decimal Budget { get; private set; }
    public decimal ActualCost { get; private set; }
    public decimal CommittedCost { get; private set; }
    public decimal ForecastCost { get; private set; }
    public string Currency { get; private set; } = "USD";

    // Progress Information
    public decimal ProgressPercentage { get; private set; }
    public decimal PhysicalProgressPercentage { get; private set; }
    public ProgressMethod ProgressMethod { get; private set; }
    public WorkPackageStatus Status { get; private set; }
    public decimal? WeightFactor { get; private set; }

    // Resources
    public Guid? ResponsibleUserId { get; private set; }
    public Guid? PrimaryDisciplineId { get; private set; } // Main discipline

    // Performance
    public decimal? CPI { get; private set; } // Cost Performance Index
    public decimal? SPI { get; private set; } // Schedule Performance Index
    public decimal EarnedValue { get; private set; }
    public decimal PlannedValue { get; private set; }

    // Control
    public bool IsBaselined { get; private set; }
    public DateTime? BaselineDate { get; private set; }

    // Tags for grouping/filtering
    public string? Tags { get; private set; } // JSON array

    // Calculated Fields
    public decimal RemainingCost => Budget - ActualCost;
    public decimal CostVariance => Budget - ForecastCost;
    public decimal ScheduleVariance => PlannedValue - EarnedValue;

    // Navigation Properties
    public WBSElement WBSElement { get; private set; } = null!;
    public User? ResponsibleUser { get; private set; }
    public Discipline? PrimaryDiscipline { get; private set; }

    // Constructor for EF Core
    private WorkPackageDetails() { }

    // Constructor for creating new Work Package Details
    public WorkPackageDetails(Guid wbsElementId, ProgressMethod progressMethod)
    {
        WBSElementId = wbsElementId;
        ProgressMethod = progressMethod;
        Status = WorkPackageStatus.NotStarted;
        PlannedStartDate = DateTime.Today;
        PlannedEndDate = DateTime.Today.AddDays(30);
        PlannedDuration = 30;
        Budget = 0;
        Currency = "USD";
        ProgressPercentage = 0;
        PhysicalProgressPercentage = 0;
        ActualCost = 0;
        CommittedCost = 0;
        ForecastCost = 0;
        EarnedValue = 0;
        PlannedValue = 0;
        TotalFloat = 0;
        FreeFloat = 0;
        IsCriticalPath = false;
        IsBaselined = false;
        CreatedAt = DateTime.UtcNow;
    }

    // Domain Methods
    public void UpdateSchedule(
        DateTime plannedStart,
        DateTime plannedEnd,
        DateTime? forecastStart,
        DateTime? forecastEnd)
    {
        if (plannedEnd <= plannedStart)
            throw new ArgumentException("End date must be after start date");

        PlannedStartDate = plannedStart;
        PlannedEndDate = plannedEnd;
        PlannedDuration = (int)(plannedEnd - plannedStart).TotalDays;

        if (forecastStart.HasValue)
            ForecastStartDate = forecastStart;

        if (forecastEnd.HasValue)
            ForecastEndDate = forecastEnd;

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBudget(decimal budget, string currency)
    {
        if (budget < 0)
            throw new ArgumentException("Budget cannot be negative");

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentNullException(nameof(currency));

        Budget = budget;
        Currency = currency;
        ForecastCost = Math.Max(ForecastCost, budget);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignResponsible(Guid userId)
    {
        ResponsibleUserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignDiscipline(Guid disciplineId)
    {
        PrimaryDisciplineId = disciplineId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetWeightFactor(decimal weightFactor)
    {
        if (weightFactor < 0 || weightFactor > 100)
            throw new ArgumentException("Weight factor must be between 0 and 100");

        WeightFactor = weightFactor;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProgress(decimal percentage, decimal? physicalPercentage = null)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Progress percentage must be between 0 and 100");

        ProgressPercentage = percentage;

        if (physicalPercentage.HasValue)
        {
            if (physicalPercentage.Value < 0 || physicalPercentage.Value > 100)
                throw new ArgumentException("Physical progress percentage must be between 0 and 100");

            PhysicalProgressPercentage = physicalPercentage.Value;
        }

        // Update status based on progress
        if (percentage == 0 && !ActualStartDate.HasValue)
        {
            Status = WorkPackageStatus.NotStarted;
        }
        else if (percentage > 0 && percentage < 100)
        {
            Status = WorkPackageStatus.InProgress;
            if (!ActualStartDate.HasValue)
            {
                ActualStartDate = DateTime.UtcNow;
            }
        }
        else if (percentage == 100)
        {
            Status = WorkPackageStatus.Completed;
            if (!ActualEndDate.HasValue)
            {
                ActualEndDate = DateTime.UtcNow;
                ActualDuration = ActualStartDate.HasValue
                    ? (int)(ActualEndDate.Value - ActualStartDate.Value).TotalDays
                    : PlannedDuration;
            }
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Baseline()
    {
        BaselineStartDate = PlannedStartDate;
        BaselineEndDate = PlannedEndDate;
        BaselineDate = DateTime.UtcNow;
        IsBaselined = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateActualCost(decimal actualCost)
    {
        if (actualCost < 0)
            throw new ArgumentException("Actual cost cannot be negative");

        ActualCost = actualCost;
        UpdatePerformanceIndices();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCommittedCost(decimal committedCost)
    {
        if (committedCost < 0)
            throw new ArgumentException("Committed cost cannot be negative");

        CommittedCost = committedCost;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEarnedValue(decimal earnedValue, decimal plannedValue)
    {
        if (earnedValue < 0)
            throw new ArgumentException("Earned value cannot be negative");

        if (plannedValue < 0)
            throw new ArgumentException("Planned value cannot be negative");

        EarnedValue = earnedValue;
        PlannedValue = plannedValue;
        UpdatePerformanceIndices();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFloat(decimal totalFloat, decimal freeFloat, bool isCriticalPath)
    {
        TotalFloat = totalFloat;
        FreeFloat = freeFloat;
        IsCriticalPath = isCriticalPath;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(WorkPackageStatus status)
    {
        Status = status;

        // Update dates based on status changes
        switch (status)
        {
            case WorkPackageStatus.InProgress:
                if (!ActualStartDate.HasValue)
                {
                    ActualStartDate = DateTime.UtcNow;
                }
                break;

            case WorkPackageStatus.Completed:
                if (!ActualEndDate.HasValue)
                {
                    ActualEndDate = DateTime.UtcNow;
                    if (ActualStartDate.HasValue)
                    {
                        ActualDuration = (int)(ActualEndDate.Value - ActualStartDate.Value).TotalDays;
                    }
                }
                break;

            case WorkPackageStatus.OnHold:
            case WorkPackageStatus.Cancelled:
                // Calculate remaining duration if in progress
                if (ActualStartDate.HasValue && !ActualEndDate.HasValue)
                {
                    var elapsedDays = (int)(DateTime.UtcNow - ActualStartDate.Value).TotalDays;
                    RemainingDuration = Math.Max(0, PlannedDuration - elapsedDays);
                }
                break;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTags(string? tags)
    {
        Tags = tags;
        UpdatedAt = DateTime.UtcNow;
    }

    private void UpdatePerformanceIndices()
    {
        // Calculate Cost Performance Index (CPI)
        if (ActualCost > 0)
        {
            CPI = EarnedValue / ActualCost;
        }

        // Calculate Schedule Performance Index (SPI)
        if (PlannedValue > 0)
        {
            SPI = EarnedValue / PlannedValue;
        }

        // Update forecast cost based on CPI
        if (CPI.HasValue && CPI.Value > 0)
        {
            ForecastCost = Budget / CPI.Value;
        }
        else
        {
            ForecastCost = Budget;
        }
    }
}