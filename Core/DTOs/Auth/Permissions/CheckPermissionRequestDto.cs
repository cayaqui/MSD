// Auth/UserDto.cs
namespace Core.DTOs.Auth.Permissions;

/// <summary>
/// DTO for checking permission request
/// </summary>
public class CheckPermissionRequestDto
{
    public Guid? ProjectId { get; set; }
    public string? Permission { get; set; }
    public string? Role { get; set; }
}

