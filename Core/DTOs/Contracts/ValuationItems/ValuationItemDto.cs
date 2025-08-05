using Core.DTOs.Common;

namespace Core.DTOs.Contracts.ValuationItems;

public class ValuationItemDto : BaseDto
{
    public Guid ValuationId { get; set; }
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
}
