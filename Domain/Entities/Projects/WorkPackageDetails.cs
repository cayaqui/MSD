using Core.Enums.Cost;
using Core.Enums.Progress;
using Domain.Common;
using Domain.Entities.Cost;
using Domain.Entities.EVM;
using Domain.Entities.Progress;
using Domain.Entities.Security;
using Domain.Entities.Setup;
using System;
using System.Collections.Generic;
using System.Linq;

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
    public ICollection<WorkPackageDiscipline> Disciplines { get; private set; } = new List<WorkPackageDiscipline>();
    public ICollection<Activity> Activities { get; private set; } = new List<Activity>();
    public ICollection<CostItem> CostItems { get; private set; } = new List<CostItem>();
    public ICollection<Resource> Resources { get; private set; } = new List<Resource>();
    public ICollection<WorkPackageProgress> ProgressRecords { get; private set; } = new List<WorkPackageProgress>();
    public ICollection<CommitmentWorkPackage> CommitmentLinks { get; private set; } = new List<CommitmentWorkPackage>();

    // Constructor for EF Core
    private WorkPackageDetails() { }

    public WorkPackageDetails(Guid wbsElementId, ProgressMethod progressMethod)
    {
        WBSElementId = wbsElementId;
        ProgressMethod = progressMethod;

        Status = WorkPackageStatus.NotStarted;
        ProgressPercentage = 0;
        PhysicalProgressPercentage = 0;
        ActualCost = 0;
        CommittedCost = 0;
        Budget = 0;
        ForecastCost = 0;
        EarnedValue = 0;
        PlannedValue = 0;
        IsBaselined = false;
        IsCriticalPath = false;
        TotalFloat = 0;
        FreeFloat = 0;

        PlannedStartDate = DateTime.UtcNow;
        PlannedEndDate = DateTime.UtcNow.AddDays(1);
        PlannedDuration = 1;

        CreatedAt = DateTime.UtcNow;
    }

    // Domain Methods
    public void UpdateSchedule(DateTime plannedStartDate, DateTime plannedEndDate)
    {
        if (IsBaselined)
            throw new InvalidOperationException("Cannot update schedule for baselined work package");

        if (plannedEndDate <= plannedStartDate)
            throw new ArgumentException("End date must be after start date");

        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        PlannedDuration = (plannedEndDate - plannedStartDate).Days;

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBudget(decimal budget, string currency)
    {
        if (IsBaselined)
            throw new InvalidOperationException("Cannot update budget for baselined work package");

        if (budget < 0)
            throw new ArgumentException("Budget cannot be negative");

        Budget = budget;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        ForecastCost = budget;

        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignResponsible(Guid userId)
    {
        ResponsibleUserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddDiscipline(Guid disciplineId, decimal estimatedHours, bool isPrimary = false)
    {
        if (Disciplines.Any(d => d.DisciplineId == disciplineId))
            throw new InvalidOperationException("Discipline already assigned to this work package");

        var wpDiscipline = new WorkPackageDiscipline(Id, disciplineId, estimatedHours, isPrimary);
        Disciplines.Add(wpDiscipline);

        // Si es la primera o es primaria, actualizar DisciplineId principal
        if (!PrimaryDisciplineId.HasValue || isPrimary)
        {
            PrimaryDisciplineId = disciplineId;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveDiscipline(Guid disciplineId)
    {
        var discipline = Disciplines.FirstOrDefault(d => d.DisciplineId == disciplineId);
        if (discipline != null)
        {
            Disciplines.Remove(discipline);

            // Si era la disciplina principal, asignar otra
            if (PrimaryDisciplineId == disciplineId)
            {
                PrimaryDisciplineId = Disciplines.FirstOrDefault()?.DisciplineId;
            }

            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void Start()
    {
        if (Status != WorkPackageStatus.NotStarted)
            throw new InvalidOperationException("Work package has already started");

        Status = WorkPackageStatus.InProgress;
        ActualStartDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProgress(decimal percentage, decimal actualCost, decimal committedCost)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentOutOfRangeException(nameof(percentage), "Progress must be between 0 and 100");

        ProgressPercentage = percentage;
        ActualCost = actualCost;
        CommittedCost = committedCost;

        // Calculate Earned Value
        EarnedValue = Budget * (percentage / 100);

        // Update forecast
        if (percentage > 0)
        {
            ForecastCost = actualCost / (percentage / 100);
        }

        // Calculate performance indices
        if (ActualCost > 0)
            CPI = EarnedValue / ActualCost;

        if (PlannedValue > 0)
            SPI = EarnedValue / PlannedValue;

        // Update status based on progress
        if (percentage == 100 && Status == WorkPackageStatus.InProgress)
        {
            Complete();
        }
        else if (percentage > 0 && Status == WorkPackageStatus.NotStarted)
        {
            Start();
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status == WorkPackageStatus.Completed)
            return;

        Status = WorkPackageStatus.Completed;
        ActualEndDate = DateTime.UtcNow;
        ProgressPercentage = 100;
        PhysicalProgressPercentage = 100;

        if (ActualStartDate.HasValue)
        {
            ActualDuration = (ActualEndDate.Value - ActualStartDate.Value).Days;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Hold()
    {
        if (Status != WorkPackageStatus.InProgress)
            throw new InvalidOperationException("Only in-progress work packages can be put on hold");

        Status = WorkPackageStatus.OnHold;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Resume()
    {
        if (Status != WorkPackageStatus.OnHold)
            throw new InvalidOperationException("Only on-hold work packages can be resumed");

        Status = WorkPackageStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (Status == WorkPackageStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed work packages");

        Status = WorkPackageStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Baseline()
    {
        if (IsBaselined)
            throw new InvalidOperationException("Work package is already baselined");

        IsBaselined = true;
        BaselineDate = DateTime.UtcNow;
        BaselineStartDate = PlannedStartDate;
        BaselineEndDate = PlannedEndDate;

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFloat(decimal totalFloat, decimal freeFloat, bool isCritical)
    {
        TotalFloat = totalFloat;
        FreeFloat = freeFloat;
        IsCriticalPath = isCritical;

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePlannedValue(decimal plannedValue)
    {
        PlannedValue = plannedValue;

        if (PlannedValue > 0)
            SPI = EarnedValue / PlannedValue;

        UpdatedAt = DateTime.UtcNow;
    }

    public void SetWeightFactor(decimal weight)
    {
        if (weight < 0 || weight > 1)
            throw new ArgumentException("Weight factor must be between 0 and 1");

        WeightFactor = weight;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return;

        var tags = GetTags().ToList();
        if (!tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            tags.Add(tag);
            Tags = System.Text.Json.JsonSerializer.Serialize(tags);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveTag(string tag)
    {
        var tags = GetTags().ToList();
        if (tags.Remove(tag))
        {
            Tags = System.Text.Json.JsonSerializer.Serialize(tags);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public IEnumerable<string> GetTags()
    {
        if (string.IsNullOrWhiteSpace(Tags))
            return Enumerable.Empty<string>();

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(Tags) ?? new List<string>();
        }
        catch
        {
            return Enumerable.Empty<string>();
        }
    }

    // Helper methods
    public decimal GetTotalEstimatedHours()
    {
        return Disciplines.Sum(d => d.EstimatedHours);
    }

    public decimal GetTotalActualHours()
    {
        return Disciplines.Sum(d => d.ActualHours);
    }

    public bool IsOverBudget() => ActualCost > Budget;

    public bool IsBehindSchedule() => ForecastEndDate > PlannedEndDate;

    public int GetDelayDays()
    {
        if (!ForecastEndDate.HasValue || ForecastEndDate <= PlannedEndDate)
            return 0;

        return (ForecastEndDate.Value - PlannedEndDate).Days;
    }
}