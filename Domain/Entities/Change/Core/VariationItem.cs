using Domain.Common;

namespace Domain.Entities.Change.Core;

/// <summary>
/// Variation line items for detailed breakdown
/// </summary>
public class VariationItem : BaseEntity
{
    public Guid VariationId { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Unit { get; private set; }
    public decimal? OriginalQuantity { get; private set; }
    public decimal? RevisedQuantity { get; private set; }
    public decimal? QuantityChange => RevisedQuantity - OriginalQuantity;
    public decimal UnitRate { get; private set; }
    public decimal Amount { get; private set; }
    public string? Remarks { get; private set; }

    public Variation Variation { get; private set; } = null!;

    private VariationItem() { }

    public VariationItem(
        Guid variationId,
        string itemCode,
        string description,
        decimal unitRate,
        decimal amount
    )
    {
        VariationId = variationId;
        ItemCode = itemCode ?? throw new ArgumentNullException(nameof(itemCode));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        UnitRate = unitRate;
        Amount = amount;
        CreatedAt = DateTime.UtcNow;
    }
}
