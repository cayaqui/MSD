namespace Core.DTOs.Auth.Roles;

/// <summary>
/// DTO for updating a role
/// </summary>
public class UpdateRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}