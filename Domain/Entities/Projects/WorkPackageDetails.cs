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
    
    // Navigation Properties
    public WBSElement WBSElement { get; private set; } = null!;
    public User? ResponsibleUser { get; private set; }
    public Discipline? PrimaryDiscipline { get; private set; }
    
    // Constructor for EF Core
    private WorkPackageDetails() { }
    
    public WorkPackageDetails(
        Guid wbsElementId,
        DateTime plannedStartDate,
        DateTime plannedEndDate,
        decimal budget,
        ProgressMethod progressMethod = ProgressMethod.Manual)
    {
        WBSElementId = wbsElementId;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        PlannedDuration = (plannedEndDate - plannedStartDate).Days;
        Budget = budget;
        ForecastCost = budget;
        ProgressMethod = progressMethod;
        Status = WorkPackageStatus.NotStarted;
        Currency = "USD";
    }
    
    // Domain Methods
    public void Start(string startedBy)
    {
        if (Status != WorkPackageStatus.NotStarted)
            throw new InvalidOperationException("Work Package can only be started from NotStarted status");
            
        Status = WorkPackageStatus.InProgress;
        ActualStartDate = DateTime.UtcNow;
        UpdatedBy = startedBy;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Complete(string completedBy)
    {
        if (Status != WorkPackageStatus.InProgress)
            throw new InvalidOperationException("Work Package can only be completed from InProgress status");
            
        Status = WorkPackageStatus.Completed;
        ActualEndDate = DateTime.UtcNow;
        ActualDuration = ActualStartDate.HasValue ? 
            (ActualEndDate.Value - ActualStartDate.Value).Days : PlannedDuration;
        ProgressPercentage = 100;
        PhysicalProgressPercentage = 100;
        UpdatedBy = completedBy;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateProgress(
        decimal progressPercentage, 
        decimal? physicalProgressPercentage,
        decimal actualCost,
        string updatedBy)
    {
        if (progressPercentage < 0 || progressPercentage > 100)
            throw new ArgumentException("Progress percentage must be between 0 and 100");
            
        ProgressPercentage = progressPercentage;
        PhysicalProgressPercentage = physicalProgressPercentage ?? progressPercentage;
        ActualCost = actualCost;
        
        // Calculate earned value
        EarnedValue = Budget * (progressPercentage / 100);
        
        // Update forecast
        if (progressPercentage > 0)
        {
            var costPercentage = actualCost / EarnedValue;
            ForecastCost = Budget * costPercentage;
        }
        
        // Calculate performance indices
        if (actualCost > 0)
            CPI = EarnedValue / actualCost;
            
        if (PlannedValue > 0)
            SPI = EarnedValue / PlannedValue;
            
        // Update remaining duration
        if (ActualStartDate.HasValue && progressPercentage < 100)
        {
            var elapsedDays = (DateTime.UtcNow - ActualStartDate.Value).Days;
            var progressRate = progressPercentage / 100;
            if (progressRate > 0)
            {
                var estimatedTotalDuration = elapsedDays / progressRate;
                RemainingDuration = (int)(estimatedTotalDuration - elapsedDays);
                ForecastEndDate = DateTime.UtcNow.AddDays(RemainingDuration.Value);
            }
        }
        
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateSchedule(
        DateTime plannedStartDate,
        DateTime plannedEndDate,
        string updatedBy)
    {
        if (IsBaselined)
            throw new InvalidOperationException("Cannot update schedule for baselined Work Package");
            
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        PlannedDuration = (plannedEndDate - plannedStartDate).Days;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Baseline(string baselinedBy)
    {
        if (IsBaselined)
            throw new InvalidOperationException("Work Package is already baselined");
            
        IsBaselined = true;
        BaselineDate = DateTime.UtcNow;
        BaselineStartDate = PlannedStartDate;
        BaselineEndDate = PlannedEndDate;
        UpdatedBy = baselinedBy;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void AssignResponsibility(Guid userId, Guid? disciplineId, string assignedBy)
    {
        ResponsibleUserId = userId;
        PrimaryDisciplineId = disciplineId;
        UpdatedBy = assignedBy;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateCosts(decimal committedCost, decimal forecastCost, string updatedBy)
    {
        CommittedCost = committedCost;
        ForecastCost = forecastCost;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateFloat(decimal totalFloat, decimal freeFloat, bool isCriticalPath)
    {
        TotalFloat = totalFloat;
        FreeFloat = freeFloat;
        IsCriticalPath = isCriticalPath;
        UpdatedAt = DateTime.UtcNow;
    }
    
    // Calculations
    public decimal GetCostVariance() => Budget - ActualCost;
    public decimal GetScheduleVariance() => PlannedValue - EarnedValue;
    public bool IsOverBudget() => ActualCost > Budget;
    public bool IsDelayed() => ForecastEndDate > PlannedEndDate || 
                               (DateTime.UtcNow > PlannedEndDate && Status != WorkPackageStatus.Completed);
}