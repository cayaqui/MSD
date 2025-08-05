using Core.DTOs.Common;
using Core.Enums.Cost;

namespace Core.DTOs.Cost;

/// <summary>
/// Query parameters specific to EVM-related endpoints
/// </summary>
public class EVMQueryParameters : QueryParameters
{
    /// <summary>
    /// Filter by period type
    /// </summary>
    public EVMPeriodType? PeriodType { get; set; }

    /// <summary>
    /// Filter by EVM status
    /// </summary>
    public EVMStatus? Status { get; set; }

    /// <summary>
    /// Filter by approval status
    /// </summary>
    public bool? IsApproved { get; set; }

    /// <summary>
    /// Filter by year
    /// </summary>
    public int? Year { get; set; }

    /// <summary>
    /// Filter by month
    /// </summary>
    public int? Month { get; set; }
}