namespace Domain.Entities.Auth.Security
{
    /// <summary>
    /// User entity representing authenticated users in the system
    /// </summary>
    public class User : BaseEntity, ISoftDelete
    {
        // Private constructor for EF
        private User()
        {
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
        public string? Phone { get; set; }
        public string? CompanyId { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsActive { get; private set; }
        
        // System role for simplified authorization
        public string? SystemRole { get; set; }
        
        // Display name property for compatibility
        public string DisplayName => Name;


        // Login Tracking
        public DateTime? LastLoginAt { get; set; }
        public int LoginCount { get; set; }

        // Soft delete properties
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Navigation properties
        public virtual ICollection<ProjectTeamMember> ProjectTeamMembers { get; private set; }

        public bool HasActiveProjects() => ProjectTeamMembers?.Any(ptm => ptm.IsActive) ?? false;
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

        public void UpdateEntraId(string entraId)
        {
            if (string.IsNullOrWhiteSpace(entraId))
                throw new ArgumentException("EntraId cannot be empty.", nameof(entraId));

            EntraId = entraId;
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

        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            LoginCount++;
            UpdatedAt = DateTime.UtcNow;
        }

        // Check if user is support/admin based on email domain
        public bool IsSupport()
        {
            return Email.EndsWith("@koffguerrero.com", StringComparison.OrdinalIgnoreCase) ||
                   Email.EndsWith("@projectsolutions.cl", StringComparison.OrdinalIgnoreCase);
        }

        // Helper methods
        private static string ExtractGivenName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return string.Empty;

            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[0] : string.Empty;
        }

        private static string ExtractSurname(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return string.Empty;

            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;
        }

        public void UpdateAzureProfile(AzureAdUser azureUser)
        {
            // Update properties based on Azure AD user data
            if (azureUser == null)
                throw new ArgumentNullException(nameof(azureUser));
            if (!string.IsNullOrWhiteSpace(azureUser.GivenName))
                {
                GivenName = azureUser.GivenName;
            }
            if (!string.IsNullOrWhiteSpace(azureUser.Surname))
            {
                Surname = azureUser.Surname;
            }
            if (!string.IsNullOrWhiteSpace(azureUser.MobilePhone))
            {
                PhoneNumber = azureUser.MobilePhone;
            }
            if (!string.IsNullOrWhiteSpace(azureUser.JobTitle))
            {
                JobTitle = azureUser.JobTitle;
            }
            if (!string.IsNullOrWhiteSpace(azureUser.PreferredLanguage))
            {
                PreferredLanguage = azureUser.PreferredLanguage;
            }
        }

        public void Restore()
        {
            // Restore soft-deleted user
            IsDeleted = false;
            DeletedAt = null;
            DeletedBy = null;
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}