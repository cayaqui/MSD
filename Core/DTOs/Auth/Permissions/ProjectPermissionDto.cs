// Core.DTOs/Auth/UserPermissionsDto.cs
namespace Core.DTOs.Auth.Permissions;

/// <summary>
/// DTO for project-specific permissions
/// </summary>
public class ProjectPermissionDto
{
    /// <summary>
    /// Project ID
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Project name
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Project code
    /// </summary>
    public string ProjectCode { get; set; } = string.Empty;

    /// <summary>
    /// User's role in the project
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// List of permissions in this project
    /// </summary>
    public List<string> Permissions { get; set; } = new();

    /// <summary>
    /// Is the user active in this project
    /// </summary>
    public bool IsActive { get; set; }
}