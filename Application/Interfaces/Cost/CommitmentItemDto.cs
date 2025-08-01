using Core.Enums.Cost;

namespace Application.Services.Interfaces;

/// <summary>
/// DTO for commitment items
/// </summary>
public class CommitmentItemDto
{
    public Guid Id { get; set; }
    public int ItemNumber { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? DetailedDescription { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public string Currency { get; set; } = string.Empty;
    public CommitmentItemStatus Status { get; set; }
    public decimal DeliveredQuantity { get; set; }
    public decimal InvoicedQuantity { get; set; }
    public decimal InvoicedAmount { get; set; }
    public DateTime? RequiredDate { get; set; }
    public DateTime? PromisedDate { get; set; }
    public string? MaterialCode { get; set; }
}
