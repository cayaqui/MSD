namespace Core.DTOs.Common;

/// <summary>
/// Filter parameters for date range queries
/// </summary>
public class DateRangeFilter
{
    /// <summary>
    /// Start date (inclusive)
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date (inclusive)
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Validates the date range
    /// </summary>
    public bool IsValid()
    {
        if (!StartDate.HasValue || !EndDate.HasValue)
            return true;

        return StartDate.Value <= EndDate.Value;
    }
}
