namespace Infrastructure.Data;

/// <summary>
/// Provides current user context for audit purposes
/// </summary>
public interface IUserContext
{
    string? CurrentUserId { get; }
}

/// <summary>
/// Implementation of IUserContext that gets the user ID from ICurrentUserService
/// </summary>
public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? CurrentUserId => _httpContextAccessor.HttpContext?.User?.FindFirst("oid")?.Value 
        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
}