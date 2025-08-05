namespace Core.ValidationResults;

public class PermissionValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public Dictionary<string, List<string>> MissingPermissions { get; set; } = new();
    public Dictionary<string, List<string>> DuplicatePermissions { get; set; } = new();
}