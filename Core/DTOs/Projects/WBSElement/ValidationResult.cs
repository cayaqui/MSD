namespace Core.DTOs.Projects.WBSElement;

/// <summary>
/// Result of WBS code validation
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> Suggestions { get; set; } = new();
}