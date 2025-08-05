using Domain.Common;
using Domain.Entities.Cost.Core;
using Domain.Entities.Organization.Core;
using Core.Enums.Cost;
using Domain.Entities.WBS;

namespace Domain.Entities.Cost.Control;

/// <summary>
/// Cost Item entity for tracking actual costs
/// </summary>
public class CostItem : BaseEntity, IAuditable, ISoftDelete
{
    // Basic Information
    public Guid ProjectId { get; private set; }
    public Guid? WBSElementId { get; private set; }
    public Guid? ControlAccountId { get; private set; }
    public Guid? CBSId { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // Cost Classification
    public CostType Type { get; private set; }
    public CostCategory Category { get; private set; }
    public string? AccountCode { get; private set; } // Accounting code
    public string? CostCenter { get; private set; }

    // Financial Information
    public decimal PlannedCost { get; private set; }
    public decimal ActualCost { get; private set; }
    public decimal Variance => ActualCost - PlannedCost;
    public decimal CostVariancePercentage => PlannedCost > 0 ? Variance / PlannedCost * 100 : 0;
    public decimal CommittedCost { get; private set; }
    public decimal ForecastCost { get; private set; }
    public string Currency { get; private set; } = "USD";
    public decimal ExchangeRate { get; private set; } = 1.0m;

    // Reference Information
    public string? ReferenceType { get; private set; } // PO, Contract, Invoice, etc.
    public string? ReferenceNumber { get; private set; }
    public DateTime? TransactionDate { get; private set; }
    public string? VendorId { get; private set; }

    // Status
    public CostItemStatus Status { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApprovedBy { get; private set; }

    // Navigation Properties
    public Project Project { get; private set; } = null!;
    public WBSElement? WBSElement { get; private set; }
    public ControlAccount? ControlAccount { get; private set; }
    public CBS? CBS { get; private set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Constructor for EF Core
    private CostItem() { }

    public CostItem(
        Guid projectId,
        string itemCode,
        string description,
        CostType type,
        CostCategory category,
        decimal plannedCost
    )
    {
        ProjectId = projectId;
        ItemCode = itemCode ?? throw new ArgumentNullException(nameof(itemCode));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Type = type;
        Category = category;
        PlannedCost = plannedCost;
        ForecastCost = plannedCost;

        Status = CostItemStatus.Planned;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    // Methods
    public void UpdatePlannedCost(decimal amount)
    {
        PlannedCost = amount;
        ForecastCost = Math.Max(ActualCost, amount);
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void RecordActualCost(
        decimal amount,
        DateTime transactionDate,
        string? referenceType,
        string? referenceNumber
    )
    {
        ActualCost = amount;
        TransactionDate = transactionDate;
        ReferenceType = referenceType;
        ReferenceNumber = referenceNumber;
        Status = CostItemStatus.Actual;

        UpdateForecast();
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void RecordCommitment(decimal amount, string referenceType, string referenceNumber)
    {
        CommittedCost = amount;
        ReferenceType = referenceType ?? throw new ArgumentNullException(nameof(referenceType));
        ReferenceNumber =
            referenceNumber ?? throw new ArgumentNullException(nameof(referenceNumber));
        Status = CostItemStatus.Committed;

        UpdateForecast();
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void Approve(string userId)
    {
        IsApproved = true;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToWBSElement(Guid wbsElementId)
    {
        WBSElementId = wbsElementId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToControlAccount(Guid controlAccountId)
    {
        ControlAccountId = controlAccountId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToCBS(Guid cbsId)
    {
        CBSId = cbsId;
        UpdatedAt = DateTime.UtcNow;
    }

    private void UpdateForecast()
    {
        ForecastCost = Math.Max(ActualCost + CommittedCost, PlannedCost);
    }

    private void Validate()
    {
        if (PlannedCost < 0 || ActualCost < 0 || CommittedCost < 0 || ForecastCost < 0)
            throw new ArgumentException("Cost values cannot be negative");
    }
}
