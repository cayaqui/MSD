using System;
using Domain.Common;
using Domain.Entities.Projects;

namespace Domain.Entities.Cost;

/// <summary>
/// Mapeo entre elementos del WBS y CBS para distribución de costos
/// </summary>
public class WBSCBSMapping : BaseEntity
{
    public Guid WBSElementId { get; private set; }
    public Guid CBSId { get; private set; }
    public decimal AllocationPercentage { get; private set; }
    public bool IsPrimary { get; private set; }

    // Additional allocation info
    public string? AllocationBasis { get; private set; } // e.g., "Direct Labor Hours", "Material Cost"
    public DateTime EffectiveDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    // Navigation
    public WBSElement WBSElement { get; private set; } = null!;
    public CBS CBS { get; private set; } = null!;

    private WBSCBSMapping() { } // EF Core

    public WBSCBSMapping(Guid wbsElementId, Guid cbsId, decimal allocationPercentage, bool isPrimary = false)
    {
        WBSElementId = wbsElementId;
        CBSId = cbsId;
        AllocationPercentage = allocationPercentage;
        IsPrimary = isPrimary;
        EffectiveDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    public void UpdateAllocation(decimal percentage, string? basis = null)
    {
        AllocationPercentage = percentage;
        if (!string.IsNullOrWhiteSpace(basis))
        {
            AllocationBasis = basis;
        }
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void SetEndDate(DateTime endDate)
    {
        if (endDate <= EffectiveDate)
            throw new ArgumentException("End date must be after effective date");

        EndDate = endDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAsPrimary()
    {
        IsPrimary = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (AllocationPercentage < 0 || AllocationPercentage > 100)
            throw new ArgumentException("Allocation percentage must be between 0 and 100");
    }

    public bool IsActive() => !EndDate.HasValue || EndDate.Value > DateTime.UtcNow;
}