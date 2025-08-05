using Domain.Common;

namespace Domain.Entities.Contracts.Core;

public class ValuationItem : BaseAuditableEntity
{
    public Guid ValuationId { get; set; }
    public virtual Valuation Valuation { get; set; } = null!;
    
    public string ItemCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    
    public decimal ContractQuantity { get; set; }
    public decimal UnitRate { get; set; }
    public decimal ContractAmount { get; set; }
    
    public decimal PreviousQuantity { get; set; }
    public decimal PreviousAmount { get; set; }
    
    public decimal CurrentQuantity { get; set; }
    public decimal CurrentAmount { get; set; }
    
    public decimal TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    
    public decimal PercentageComplete { get; set; }
    
    public string WorkPackageCode { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    // Methods
    public void CalculateAmounts()
    {
        ContractAmount = ContractQuantity * UnitRate;
        CurrentAmount = CurrentQuantity * UnitRate;
        TotalQuantity = PreviousQuantity + CurrentQuantity;
        TotalAmount = PreviousAmount + CurrentAmount;
        
        if (ContractQuantity > 0)
        {
            PercentageComplete = TotalQuantity / ContractQuantity * 100;
        }
    }
    
    public void UpdateQuantity(decimal quantity)
    {
        CurrentQuantity = quantity;
        CalculateAmounts();
    }
}