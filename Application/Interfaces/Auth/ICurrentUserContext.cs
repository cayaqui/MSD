namespace Application.Interfaces.Auth
{

    /// <summary>
    /// Mutable context for storing current user information.
    /// This is populated by the authentication middleware.
    /// </summary>
    public interface ICurrentUserContext
    {
        string? UserId { get; set; }
        string? UserName { get; set; }
        string? Email { get; set; }
        bool IsAuthenticated { get; set; }
    }
}