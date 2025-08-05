using Core.DTOs.Common;
using Core.Enums.Cost;

namespace Core.DTOs.Cost;

/// <summary>
/// Query parameters specific to cost-related endpoints
/// </summary>
public class CostQueryParameters : QueryParameters
{
    /// <summary>
    /// Filter by cost type
    /// </summary>
    public CostType? Type { get; set; }

    /// <summary>
    /// Filter by cost category
    /// </summary>
    public CostCategory? Category { get; set; }

    /// <summary>
    /// Filter by cost item status
    /// </summary>
    public CostItemStatus? Status { get; set; }

    /// <summary>
    /// Filter by control account ID
    /// </summary>
    public Guid? ControlAccountId { get; set; }

    /// <summary>
    /// Filter by WBS element ID
    /// </summary>
    public Guid? WBSElementId { get; set; }

    /// <summary>
    /// Filter by approval status
    /// </summary>
    public bool? IsApproved { get; set; }
}