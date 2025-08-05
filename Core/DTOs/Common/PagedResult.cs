namespace Core.DTOs.Common;

/// <summary>
/// Paginated result container
/// </summary>
public class PagedResult<T> where T : class
{
    /// <summary>
    /// Items in the current page
    /// </summary>
    public IReadOnlyList<T> Items { get; set; }

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount => Items.Count;

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Indicates if there's a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indicates if there's a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Gets the starting item number for the current page
    /// </summary>
    public int StartItemNumber => TotalCount > 0 ? ((PageNumber - 1) * PageSize) + 1 : 0;

    /// <summary>
    /// Gets the ending item number for the current page
    /// </summary>
    public int EndItemNumber => Math.Min(PageNumber * PageSize, TotalCount);

    /// <summary>
    /// Default constructor
    /// </summary>
    public PagedResult()
    {
        Items = new List<T>();
    }

    /// <summary>
    /// Constructor with parameters
    /// </summary>
    public PagedResult(IEnumerable<T> items, int pageNumber, int pageSize)
    {
        Items = items?.ToList() ?? new List<T>();
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = pageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)pageSize) : 0;
    }

    /// <summary>
    /// Creates an empty paged result
    /// </summary>
    public static PagedResult<T> Empty(int pageNumber = 1, int pageSize = 20)
    {
        return new PagedResult<T>
        {
            Items = new List<T>(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = 0
        };
    }
}