using Application.Interfaces.Auth;

namespace Application.Services.Auth
{
    /// <summary>
    /// Implementation of the current user context.
    /// This is registered as Scoped to maintain state per request.
    /// </summary>
    public class CurrentUserContext : ICurrentUserContext
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}