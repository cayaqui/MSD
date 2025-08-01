namespace Core.DTOs.Cost.Commitments;

/// <summary>
/// DTO for updating commitment items
/// </summary>
public class UpdateCommitmentItemDto
{
    public string? Description { get; set; }
    public string? DetailedDescription { get; set; }
    public string? Specifications { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? TaxRate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public DateTime? PromisedDate { get; set; }
    public string? DeliveryLocation { get; set; }
    public string? DrawingNumber { get; set; }
    public string? MaterialCode { get; set; }
    public string? VendorItemCode { get; set; }
}
