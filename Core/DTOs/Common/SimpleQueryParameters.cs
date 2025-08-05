namespace Core.DTOs.Common;

/// <summary>
/// Simplified query parameters for minimal API endpoints
/// </summary>
public class SimpleQueryParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 20;
    private string? _searchTerm;

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>
    /// Search term for filtering
    /// </summary>
    public string? SearchTerm
    {
        get => _searchTerm;
        set => _searchTerm = value?.Trim();
    }

    /// <summary>
    /// Field name to sort by
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (asc/desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    /// <summary>
    /// Include soft-deleted records
    /// </summary>
    public bool IncludeDeleted { get; set; } = false;

    /// <summary>
    /// Include inactive records
    /// </summary>
    public bool IncludeInactive { get; set; } = false;

    /// <summary>
    /// Checks if sorting is ascending
    /// </summary>
    public bool IsAscending => SortDirection?.ToLower() != "desc";

    /// <summary>
    /// Gets the skip count for pagination
    /// </summary>
    public int Skip => (PageNumber - 1) * PageSize;

    /// <summary>
    /// Validates the parameters
    /// </summary>
    public bool IsValid()
    {
        return PageNumber > 0 && PageSize > 0 && PageSize <= MaxPageSize;
    }

    /// <summary>
    /// Converts to full QueryParameters
    /// </summary>
    public QueryParameters ToQueryParameters()
    {
        return new QueryParameters
        {
            PageNumber = PageNumber,
            PageSize = PageSize,
            SearchTerm = SearchTerm,
            SortBy = SortBy,
            SortDirection = SortDirection,
            IncludeDeleted = IncludeDeleted,
            IncludeInactive = IncludeInactive
        };
    }
}