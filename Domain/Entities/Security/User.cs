using Domain.Common;

namespace Domain.Entities.Security;

/// <summary>
/// User entity representing authenticated users in the system
/// </summary>
public class User : BaseEntity, ISoftDelete
{
    // Private constructor for EF
    private User()
    {
        //UserRoles = new HashSet<UserRole>();
        ProjectTeamMembers = new HashSet<ProjectTeamMember>();
    }

    public User(string entraId, string email, string name)
        : this()
    {
        EntraId = entraId;
        Email = email.ToLowerInvariant();
        Name = name;
        IsActive = true;
        GivenName = ExtractGivenName(name);
        Surname = ExtractSurname(name);
        LoginCount = 0;
    }

    // Core Properties
    public string EntraId { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? GivenName { get; private set; }
    public string? Surname { get; private set; }
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public string? CompanyId { get; set; }
    public string? PreferredLanguage { get; set; }
    public bool IsActive { get; private set; }

    // Login Tracking
    public DateTime? LastLoginAt { get; set; }
    public int LoginCount { get; set; }

    // Soft delete properties
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation properties
    //public virtual ICollection<UserRole> UserRoles { get; private set; }
    public virtual ICollection<ProjectTeamMember> ProjectTeamMembers { get; private set; }

    // Methods
    public void UpdateProfile(string name, string? givenName = null, string? surname = null)
    {
        Name = name;
        GivenName = givenName ?? ExtractGivenName(name);
        Surname = surname ?? ExtractSurname(name);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePersonalInfo(
        string? givenName,
        string? surname,
        string? phoneNumber,
        string? jobTitle,
        string? preferredLanguage
    )
    {
        GivenName = givenName;
        Surname = surname;
        PhoneNumber = phoneNumber;
        JobTitle = jobTitle;
        PreferredLanguage = preferredLanguage;

        // Update Name based on GivenName and Surname
        if (!string.IsNullOrWhiteSpace(givenName) || !string.IsNullOrWhiteSpace(surname))
        {
            Name = $"{givenName} {surname}".Trim();
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
            throw new ArgumentException("Email cannot be empty.", nameof(newEmail));

        Email = newEmail.ToLowerInvariant();
        UpdatedAt = DateTime.UtcNow;
    }
    public void UpdateEntraId(string newEntraId)
    {
        if (string.IsNullOrWhiteSpace(newEntraId))
            throw new ArgumentException("Entra ID cannot be empty.", nameof(newEntraId));
        EntraId = newEntraId;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        IsActive = false;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        LoginCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAzureProfile(AzureAdUser azureUser)
    {
        if (azureUser == null)
            throw new ArgumentNullException(nameof(azureUser));

        EntraId = azureUser.Id ?? EntraId;
        Email = azureUser.Mail?.ToLowerInvariant() ?? Email;
        Name = azureUser.DisplayName ?? Name;
        GivenName = azureUser.GivenName ?? GivenName;
        Surname = azureUser.Surname ?? Surname;
        PhoneNumber = azureUser.MobilePhone ?? PhoneNumber;
        JobTitle = azureUser.JobTitle ?? JobTitle;
        PreferredLanguage = azureUser.PreferredLanguage ?? PreferredLanguage;
        UpdatedAt = DateTime.UtcNow;
    }

    // Helper methods
    private static string? ExtractGivenName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return null;

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : null;
    }

    private static string? ExtractSurname(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return null;

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : null;
    }

    /// <summary>
    /// Gets a display name for the user
    /// </summary>
    public string GetDisplayName()
    {
        if (!string.IsNullOrWhiteSpace(GivenName) && !string.IsNullOrWhiteSpace(Surname))
        {
            return $"{GivenName} {Surname}";
        }

        if (!string.IsNullOrWhiteSpace(Name))
        {
            return Name;
        }

        return Email.Split('@')[0];
    }

    /// <summary>
    /// Checks if user has any active project membership
    /// </summary>
    public bool HasActiveProjects()
    {
        return ProjectTeamMembers.Any(ptm =>
            ptm.IsActive &&
            (!ptm.EndDate.HasValue || ptm.EndDate.Value > DateTime.UtcNow));
    }

    /// <summary>
    /// Gets user's active projects count
    /// </summary>
    public int GetActiveProjectsCount()
    {
        return ProjectTeamMembers.Count(ptm =>
            ptm.IsActive &&
            (!ptm.EndDate.HasValue || ptm.EndDate.Value > DateTime.UtcNow));
    }

    /// <summary>
    /// Checks if user has a specific role in any project
    /// </summary>
    public bool HasRoleInAnyProject(string role)
    {
        return ProjectTeamMembers.Any(ptm =>
            ptm.Role == role &&
            ptm.IsActive &&
            (!ptm.EndDate.HasValue || ptm.EndDate.Value > DateTime.UtcNow));
    }

    /// <summary>
    /// Gets user's role in a specific project
    /// </summary>
    public string? GetProjectRole(Guid projectId)
    {
        return ProjectTeamMembers
            .FirstOrDefault(ptm =>
                ptm.ProjectId == projectId &&
                ptm.IsActive &&
                (!ptm.EndDate.HasValue || ptm.EndDate.Value > DateTime.UtcNow))
            ?.Role;
    }

    public bool IsAdmin()
    {
        // Assuming "Admin" is a specific role in the system
        return ProjectTeamMembers.Any(ptm => ptm.Role == "Admin" && ptm.IsActive);
    }
    public bool IsSupport()
    {
        // Assuming "Support" is a specific role in the system
        return ProjectTeamMembers.Any(ptm => ptm.Role == "Support" && ptm.IsActive);
    }
}
