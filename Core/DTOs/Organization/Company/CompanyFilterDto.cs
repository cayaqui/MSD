using Core.DTOs.Common;

namespace Core.DTOs.Organization.Company;

/// <summary>
/// Filter DTO for company queries
/// </summary>
public class CompanyFilterDto : BaseFilterDto
{
    /// <summary>
    /// Filter by country
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Sort by field (name, code, taxid, createdat)
    /// </summary>
    public override string? SortBy { get; set; } = "name";

}