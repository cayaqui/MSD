using Domain.Common;

namespace Domain.Entities.Cost.Control;

/// <summary>
/// Actual cost record entity
/// </summary>
public class ActualCost : BaseEntity
{
    public Guid CostItemId { get; set; }
    public decimal Amount { get; set; }
    public DateTime ActualDate { get; set; }
    public string? InvoiceReference { get; set; }
    public string? Description { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
  
  
    // Navigation Properties
    public CostItem CostItem { get; set; } = null!;
}