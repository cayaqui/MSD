namespace Core.DTOs.Contracts.ValuationItems;

public class UpdateValuationItemDto
{
    public Guid Id { get; set; }
    public decimal CurrentQuantity { get; set; }
    public string Comments { get; set; } = string.Empty;
}
