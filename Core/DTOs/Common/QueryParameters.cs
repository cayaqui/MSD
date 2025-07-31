namespace Core.DTOs.Common;

//public class PagedResult<T>
//{
//    public PagedResult() {}
//    public IEnumerable<T> Items { get; set; } = new List<T>();
//    public int PageNumber { get; set; }
//    public int PageSize { get; set; }
//    public int TotalCount => Items.Count();
//    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
//    public bool HasPreviousPage => PageNumber > 1;
//    public bool HasNextPage => PageNumber < TotalPages;
//    public PagedResult(IEnumerable<T> items, int page, int pageSize)
//    {
//        Items = items;
//        PageNumber = page;
//        PageSize = pageSize;
//    }
//}


/// <summary>
/// Generic parameters for querying data with pagination, filtering, and sorting
/// </summary>
public class QueryParameters
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
    /// Additional filters as key-value pairs
    /// </summary>
    public Dictionary<string, string> Filters { get; set; } = new();

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
}
