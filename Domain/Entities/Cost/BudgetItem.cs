using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Common;
using Domain.Entities.Setup;
using Core.Enums.Cost;

namespace Domain.Entities.Cost;

/// <summary>
/// Budget Item - detailed line item in a budget
/// </summary>
public class BudgetItem : BaseEntity, IAuditable
{
    // Basic Information
    public Guid BudgetId { get; private set; }
    public Guid? ControlAccountId { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // Cost Information
    public CostType CostType { get; private set; }
    public CostCategory Category { get; private set; }
    public decimal Amount { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitRate { get; private set; }
    public string? UnitOfMeasure { get; private set; }

    // Additional Information
    public string? AccountingCode { get; private set; }
    public string? Notes { get; private set; }
    public int SortOrder { get; private set; }

    // Navigation Properties
    public Budget Budget { get; private set; } = null!;
    public ControlAccount? ControlAccount { get; private set; }
    //public WorkPackage? WorkPackage { get; private set; }

    // Constructor for EF Core
    private BudgetItem() { }

    public BudgetItem(
        Guid budgetId,
        string itemCode,
        string description,
        CostType costType,
        CostCategory category,
        decimal amount)
    {
        BudgetId = budgetId;
        ItemCode = itemCode ?? throw new ArgumentNullException(nameof(itemCode));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        CostType = costType;
        Category = category;
        Amount = amount;

        Quantity = 1;
        UnitRate = amount;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    // Methods
    public void UpdateAmount(decimal quantity, decimal unitRate)
    {
        Quantity = quantity;
        UnitRate = unitRate;
        Amount = quantity * unitRate;
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void AssignToControlAccount(Guid controlAccountId)
    {
        ControlAccountId = controlAccountId;
        UpdatedAt = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (Amount < 0)
            throw new ArgumentException("Amount cannot be negative");

        if (Quantity <= 0)
            throw new ArgumentException("Quantity must be positive");

        if (UnitRate < 0)
            throw new ArgumentException("Unit rate cannot be negative");
    }
}
