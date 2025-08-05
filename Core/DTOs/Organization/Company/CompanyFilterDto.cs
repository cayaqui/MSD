namespace Core.DTOs.Organization.Company;

/// <summary>
/// Filter DTO for company queries
/// </summary>
public class CompanyFilterDto
{
    /// <summary>
    /// Search term to filter by name, code or tax ID
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Filter by country
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Sort by field (name, code, taxid, createdat)
    /// </summary>
    public string? SortBy { get; set; } = "name";

    /// <summary>
    /// Sort in descending order
    /// </summary>
    public bool SortDescending { get; set; }
}