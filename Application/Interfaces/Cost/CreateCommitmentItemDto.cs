using Core.DTOs.Common;
using Core.DTOs.Cost;

namespace Application.Services.Interfaces;

/// <summary>
/// DTO for creating commitment items
/// </summary>
public class CreateCommitmentItemDto
{
    public int ItemNumber { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? DetailedDescription { get; set; }
    public string? Specifications { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public Guid? BudgetItemId { get; set; }
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
