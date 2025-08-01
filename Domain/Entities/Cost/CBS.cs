using Core.Enums.Cost;
using Domain.Common;
using Domain.Entities.Projects;
using System;
using System.Collections.Generic;

namespace Domain.Entities.Cost;

/// <summary>
/// Cost Breakdown Structure (CBS) entity following AACE standards
/// Represents the hierarchical structure for organizing and controlling project costs
/// </summary>
public class CBS : BaseEntity, IAuditable, ISoftDelete, IActivatable
{
    // Basic Information
    public string Code { get; private set; } = string.Empty; // e.g., "01.02.03"
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Hierarchy
    public Guid ProjectId { get; private set; }
    public Guid? ParentId { get; private set; }
    public int Level { get; private set; }
    public int SequenceNumber { get; private set; }
    public string FullPath { get; private set; } = string.Empty; // e.g., "Labor/Direct/Engineering"

    // Classification
    public CostCategory Category { get; private set; }
    public CostType CostType { get; private set; }
    public bool IsControlPoint { get; private set; } // Where costs are controlled
    public bool IsLeafNode { get; private set; } // Lowest level where costs are collected

    // Cost Account Mapping
    public string? CostAccountCode { get; private set; }
    public string? AccountingCode { get; private set; } // GL Account
    public string? CostCenter { get; private set; }

    // Estimation Class (AACE)
    public EstimateClass? EstimateClass { get; private set; }
    public decimal? EstimateAccuracyLow { get; private set; } // e.g., -15%
    public decimal? EstimateAccuracyHigh { get; private set; } // e.g., +20%

    // Budget Information
    public decimal OriginalBudget { get; private set; }
    public decimal ApprovedChanges { get; private set; }
    public decimal CurrentBudget => OriginalBudget + ApprovedChanges;
    public decimal CommittedCost { get; private set; }
    public decimal ActualCost { get; private set; }
    public decimal ForecastCost { get; private set; }
    public string Currency { get; private set; } = "USD";

    // Variance Analysis
    public decimal CostVariance => CurrentBudget - ForecastCost;
    public decimal CostVariancePercentage => CurrentBudget > 0 ? (CostVariance / CurrentBudget) * 100 : 0;

    // Allocation
    public decimal? AllocationPercentage { get; private set; } // For indirect costs
    public string? AllocationBasis { get; private set; } // e.g., "Direct Labor Hours"

    // Time-phased Budget
    public bool IsTimePhased { get; private set; }
    public string? TimePhasedData { get; private set; } // JSON array of monthly/weekly budgets

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Activatable
    public bool IsActive { get; set; }

    // Navigation Properties
    public Project Project { get; private set; } = null!;
    public CBS? Parent { get; private set; }
    public ICollection<CBS> Children { get; private set; } = new List<CBS>();
    public ICollection<CostItem> CostItems { get; private set; } = new List<CostItem>();
    public ICollection<BudgetItem> BudgetItems { get; private set; } = new List<BudgetItem>();
    public ICollection<WBSElement> WBSMappings { get; private set; } = new List<WBSElement>();

    // Constructor for EF Core
    private CBS() { }

    public CBS(
        Guid projectId,
        string code,
        string name,
        CostCategory category,
        CostType costType,
        int level,
        int sequenceNumber,
        Guid? parentId = null)
    {
        ProjectId = projectId;
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Category = category;
        CostType = costType;
        Level = level;
        SequenceNumber = sequenceNumber;
        ParentId = parentId;

        OriginalBudget = 0;
        ApprovedChanges = 0;
        CommittedCost = 0;
        ActualCost = 0;
        ForecastCost = 0;
        IsActive = true;
        IsLeafNode = true; // Default to leaf, will change if children added
        CreatedAt = DateTime.UtcNow;

        ValidateCode();
    }

    // Methods
    public void UpdateBasicInfo(string name, string? description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAsControlPoint()
    {
        IsControlPoint = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCostAccount(string? costAccountCode, string? accountingCode, string? costCenter)
    {
        CostAccountCode = costAccountCode;
        AccountingCode = accountingCode;
        CostCenter = costCenter;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetEstimateClass(EstimateClass estimateClass, decimal accuracyLow, decimal accuracyHigh)
    {
        if (accuracyLow > 0 || accuracyHigh < 0)
            throw new ArgumentException("Accuracy range must be negative for low and positive for high");

        EstimateClass = estimateClass;
        EstimateAccuracyLow = accuracyLow;
        EstimateAccuracyHigh = accuracyHigh;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBudget(decimal originalBudget)
    {
        if (originalBudget < 0)
            throw new ArgumentException("Budget cannot be negative");

        OriginalBudget = originalBudget;
        ForecastCost = originalBudget; // Initial forecast equals budget
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApproveChange(decimal changeAmount, string? reason)
    {
        ApprovedChanges += changeAmount;

        // Adjust forecast if increase
        if (changeAmount > 0)
        {
            ForecastCost += changeAmount;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateActuals(decimal committedCost, decimal actualCost, decimal forecastCost)
    {
        if (committedCost < 0 || actualCost < 0 || forecastCost < 0)
            throw new ArgumentException("Cost values cannot be negative");

        CommittedCost = committedCost;
        ActualCost = actualCost;
        ForecastCost = forecastCost;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAllocation(decimal percentage, string basis)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Allocation percentage must be between 0 and 100");

        AllocationPercentage = percentage;
        AllocationBasis = basis ?? throw new ArgumentNullException(nameof(basis));
        UpdatedAt = DateTime.UtcNow;
    }

    public void EnableTimePhasing(string timePhasedData)
    {
        IsTimePhased = true;
        TimePhasedData = timePhasedData;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFullPath(string fullPath)
    {
        FullPath = fullPath ?? throw new ArgumentNullException(nameof(fullPath));
        UpdatedAt = DateTime.UtcNow;
    }

    public CBS AddChild(
        string code,
        string name,
        CostCategory category,
        CostType costType,
        int sequenceNumber)
    {
        var child = new CBS(
            ProjectId,
            code,
            name,
            category,
            costType,
            Level + 1,
            sequenceNumber,
            Id);

        Children.Add(child);

        // Parent is no longer a leaf node
        IsLeafNode = false;

        return child;
    }

    // Calculate rollup values from children
    public void CalculateRollup()
    {
        if (!Children.Any() || IsLeafNode)
            return;

        var activeChildren = Children.Where(c => c.IsActive && !c.IsDeleted).ToList();

        if (!activeChildren.Any())
            return;

        // Roll up budgets
        OriginalBudget = activeChildren.Sum(c => c.OriginalBudget);
        ApprovedChanges = activeChildren.Sum(c => c.ApprovedChanges);

        // Roll up actuals
        CommittedCost = activeChildren.Sum(c => c.CommittedCost);
        ActualCost = activeChildren.Sum(c => c.ActualCost);
        ForecastCost = activeChildren.Sum(c => c.ForecastCost);

        UpdatedAt = DateTime.UtcNow;
    }

    // Distribute budget to children based on percentage
    public void DistributeBudget(Dictionary<Guid, decimal> childPercentages)
    {
        var totalPercentage = childPercentages.Values.Sum();
        if (Math.Abs(totalPercentage - 100) > 0.01m)
            throw new ArgumentException("Child percentages must sum to 100%");

        foreach (var child in Children.Where(c => c.IsActive && !c.IsDeleted))
        {
            if (childPercentages.TryGetValue(child.Id, out var percentage))
            {
                var childBudget = OriginalBudget * (percentage / 100);
                child.UpdateBudget(childBudget);
            }
        }

        UpdatedAt = DateTime.UtcNow;
    }

    // Validation
    private void ValidateCode()
    {
        if (!System.Text.RegularExpressions.Regex.IsMatch(Code, @"^(\d{2}\.)*\d{2}$"))
            throw new ArgumentException("CBS code must follow the pattern: 01.02.03 format");
    }

    // Helper methods
    public bool IsOverBudget() => ForecastCost > CurrentBudget;

    public bool HasChildren() => Children.Any(c => c.IsActive && !c.IsDeleted);

    public decimal GetBurnRate()
    {
        if (ActualCost == 0 || CreatedAt == UpdatedAt)
            return 0;

        var days = (UpdatedAt ?? DateTime.UtcNow).Subtract(CreatedAt).TotalDays;
        return days > 0 ? ActualCost / (decimal)days : 0;
    }

    public IEnumerable<CBS> GetAllDescendants()
    {
        foreach (var child in Children)
        {
            yield return child;
            foreach (var descendant in child.GetAllDescendants())
            {
                yield return descendant;
            }
        }
    }

    public IEnumerable<CBS> GetLeafNodes()
    {
        return GetAllDescendants().Where(c => c.IsLeafNode);
    }
}

/// <summary>
/// Estimate Class according to AACE International
/// </summary>
public enum EstimateClass
{
    Class5 = 5, // Concept Screening: -20% to -50% / +30% to +100%
    Class4 = 4, // Study/Feasibility: -15% to -30% / +20% to +50%
    Class3 = 3, // Budget Authorization: -10% to -20% / +10% to +30%
    Class2 = 2, // Control/Bid: -5% to -15% / +5% to +20%
    Class1 = 1  // Check Estimate: -3% to -10% / +3% to +15%
}