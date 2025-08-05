// Auth/UserDto.cs

// Auth/UserDto.cs
namespace Core.DTOs.Auth.Users;

/// <summary>
/// DTO for login response
/// </summary>
public class LoginResponseDto
{
    public UserDto User { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

