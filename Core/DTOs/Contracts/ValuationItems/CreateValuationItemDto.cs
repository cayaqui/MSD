namespace Core.DTOs.Contracts.ValuationItems;

public class CreateValuationItemDto
{
    public string ItemCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    
    public decimal ContractQuantity { get; set; }
    public decimal UnitRate { get; set; }
    public decimal CurrentQuantity { get; set; }
    
    public string WorkPackageCode { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
}
