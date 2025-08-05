namespace Core.DTOs.Organization.RAM;

/// <summary>
/// DTO for RAM validation result
/// </summary>
public class RAMValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}