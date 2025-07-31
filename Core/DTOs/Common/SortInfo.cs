using Core.Enums.UI;

namespace Core.DTOs.Common;

/// <summary>
/// Sort information
/// </summary>
public class SortInfo
{
    /// <summary>
    /// Field to sort by
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// Sort direction
    /// </summary>
    public SortDirection Direction { get; set; } = SortDirection.Ascending;
}
