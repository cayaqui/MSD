namespace Core.DTOs.Configuration.OBSTemplates;

public class OBSValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool HasCircularReference { get; set; }
    public bool HasDuplicateCodes { get; set; }
}