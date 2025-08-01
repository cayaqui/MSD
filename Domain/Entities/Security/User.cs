using Domain.Common;
using System;
using System.Collections.Generic;

namespace Domain.Entities.Security
{
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
            UserProjectPermissions = new HashSet<UserProjectPermission>();
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

        public string? PhotoUrl { get; set; }
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
        public virtual ICollection<UserProjectPermission> UserProjectPermissions { get; private set; }

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
            return Email.EndsWith("@mosaicmds.com", StringComparison.OrdinalIgnoreCase) ||
                   Email.EndsWith("@synapsis-usa.com", StringComparison.OrdinalIgnoreCase);
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
    }
}