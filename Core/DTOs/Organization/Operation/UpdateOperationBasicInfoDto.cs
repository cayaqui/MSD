namespace Core.DTOs.Organization.Operation;

/// <summary>
/// DTO for updating operation basic information
/// </summary>
public class UpdateOperationBasicInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
}