namespace Core.DTOs.Common;

public abstract class BaseFilterDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // Basic filters
    public string? SearchTerm { get; set; }
    public Guid? ProjectId { get; set; }
    public string SortDirection { get; set; } = "asc";
    public virtual string? SortBy { get; set; }
    public bool IncludeDeleted { get; set; } = false;
    public bool IncludeInactive { get; set; } = false;
}
