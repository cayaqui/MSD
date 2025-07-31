namespace Domain.Common;

/// <summary>
/// Represents a user from Azure AD
/// </summary>
public class AzureAdUser
{
    public string Id { get; set; } = string.Empty; // Object ID
    public string DisplayName { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string UserPrincipalName { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public string? OfficeLocation { get; set; }
    public string? MobilePhone { get; set; }
    public string? PreferredLanguage { get; set; }
    public bool AccountEnabled { get; set; }
}