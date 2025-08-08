namespace Core.DTOs.Common;

/// <summary>
/// Base class for query parameters
/// </summary>
public class BaseQueryParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 20;

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>
    /// Search term
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Sort by field
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort ascending
    /// </summary>
    public bool IsAscending { get; set; } = true;

    /// <summary>
    /// Include deleted items
    /// </summary>
    public bool IncludeDeleted { get; set; } = false;
}