namespace Core.DTOs.Configuration.OBSTemplates;

/// <summary>
/// DTO for OBS node member
/// </summary>
public class OBSMemberDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Title { get; set; }
    public string? Department { get; set; }
    public string? Role { get; set; }
    public decimal? FTEAllocation { get; set; }
    public decimal? AllocatedFTE { get; set; } // Alias for FTEAllocation
    public bool IsManager { get; set; }
    public DateTime JoinedDate { get; set; }
}
