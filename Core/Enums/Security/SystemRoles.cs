namespace Core.Enums.Security;

/// <summary>
/// System roles for authorization
/// </summary>
public static class SystemRoles
{
    public const string Admin = "Admin";
    public const string Support = "Support";
    public const string User = "User";

    /// <summary>
    /// Gets all system roles
    /// </summary>
    public static IEnumerable<string> GetAll()
    {
        yield return Admin;
        yield return Support;
        yield return User;
    }

    /// <summary>
    /// Checks if a role is valid
    /// </summary>
    public static bool IsValidRole(string role)
    {
        return GetAll().Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}