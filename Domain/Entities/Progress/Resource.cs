using System;
using Domain.Common;
using Domain.Entities.Setup;
using Core.Enums.Progress;

namespace Domain.Entities.Progress;

/// <summary>
/// Resource entity for tracking labor, equipment, and materials assigned to activities
/// </summary>
public class Resource : BaseEntity, IAuditable, ISoftDelete
{
    // Basic Information
    public string ResourceCode { get; private set; } = string.Empty;
    public string ResourceName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ResourceType Type { get; private set; }

    // Assignment Information
    public Guid ActivityId { get; private set; }
    public Guid? UserId { get; private set; } // For labor resources
    public Guid? ContractorId { get; private set; } // For subcontractor resources
    public Guid? EquipmentId { get; private set; } // For equipment resources

    // Quantity and Cost Information
    public decimal PlannedQuantity { get; private set; }
    public decimal ActualQuantity { get; private set; }
    public string? UnitOfMeasure { get; private set; }
    public decimal? UnitRate { get; private set; }
    public string Currency { get; private set; } = "USD";

    // Time Information (for labor resources)
    public decimal PlannedHours { get; private set; }
    public decimal ActualHours { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    // Calculated Fields
    public decimal PlannedCost => PlannedQuantity * (UnitRate ?? 0);
    public decimal ActualCost => ActualQuantity * (UnitRate ?? 0);
    public decimal CostVariance => PlannedCost - ActualCost;
    public decimal QuantityVariance => PlannedQuantity - ActualQuantity;
    public decimal UtilizationRate => PlannedQuantity > 0 ? (ActualQuantity / PlannedQuantity) * 100 : 0;

    // Status
    public bool IsActive { get; private set; }
    public bool IsOverAllocated { get; private set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public Activity Activity { get; private set; } = null!;
    public User? User { get; private set; }
    public Contractor? Contractor { get; private set; }

    // Constructor for EF Core
    private Resource() { }

    public Resource(
        string resourceCode,
        string resourceName,
        ResourceType type,
        Guid activityId,
        decimal plannedQuantity,
        string? unitOfMeasure)
    {
        ResourceCode = resourceCode ?? throw new ArgumentNullException(nameof(resourceCode));
        ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
        Type = type;
        ActivityId = activityId;
        PlannedQuantity = plannedQuantity;
        UnitOfMeasure = unitOfMeasure;

        ActualQuantity = 0;
        PlannedHours = 0;
        ActualHours = 0;
        IsActive = true;
        IsOverAllocated = false;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    // Methods for Labor Resources
    public void AssignUser(Guid userId, decimal plannedHours, decimal? hourlyRate = null)
    {
        if (Type != ResourceType.Labor)
            throw new InvalidOperationException("Can only assign users to labor resources");

        UserId = userId;
        PlannedHours = plannedHours;
        if (hourlyRate.HasValue)
        {
            UnitRate = hourlyRate.Value;
        }
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void AssignContractor(Guid contractorId, decimal? rate = null)
    {
        if (Type != ResourceType.Labor && Type != ResourceType.Subcontractor)
            throw new InvalidOperationException("Can only assign contractors to labor or service resources");

        ContractorId = contractorId;
        if (rate.HasValue)
        {
            UnitRate = rate.Value;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    // Methods for Equipment Resources
    public void AssignEquipment(Guid equipmentId, decimal? dailyRate = null)
    {
        if (Type != ResourceType.Equipment)
            throw new InvalidOperationException("Can only assign equipment to equipment resources");

        EquipmentId = equipmentId;
        if (dailyRate.HasValue)
        {
            UnitRate = dailyRate.Value;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    // Update Methods
    public void UpdatePlannedQuantity(decimal quantity, decimal? unitRate = null)
    {
        PlannedQuantity = quantity;
        if (unitRate.HasValue)
        {
            UnitRate = unitRate.Value;
        }
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void UpdateActualQuantity(decimal quantity)
    {
        ActualQuantity = quantity;
        UpdatedAt = DateTime.UtcNow;

        // Check for over-allocation
        if (ActualQuantity > PlannedQuantity * 1.1m) // 10% threshold
        {
            IsOverAllocated = true;
        }
    }

    public void UpdateActualHours(decimal hours)
    {
        if (Type != ResourceType.Labor)
            throw new InvalidOperationException("Can only update hours for labor resources");

        ActualHours = hours;
        ActualQuantity = hours; // For labor, quantity is hours
        UpdatedAt = DateTime.UtcNow;

        // Check for over-allocation
        if (ActualHours > PlannedHours * 1.1m) // 10% threshold
        {
            IsOverAllocated = true;
        }
    }

    public void SetSchedule(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date must be after start date");

        StartDate = startDate;
        EndDate = endDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string resourceName, string? description)
    {
        ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetUnitRate(decimal rate, string currency)
    {
        if (rate < 0)
            throw new ArgumentException("Unit rate cannot be negative");

        UnitRate = rate;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        UpdatedAt = DateTime.UtcNow;
    }

    // Private Methods
    private void Validate()
    {
        if (PlannedQuantity < 0)
            throw new ArgumentException("Planned quantity cannot be negative");

        if (ActualQuantity < 0)
            throw new ArgumentException("Actual quantity cannot be negative");

        if (PlannedHours < 0)
            throw new ArgumentException("Planned hours cannot be negative");

        if (ActualHours < 0)
            throw new ArgumentException("Actual hours cannot be negative");

        if (UnitRate.HasValue && UnitRate.Value < 0)
            throw new ArgumentException("Unit rate cannot be negative");
    }
}
