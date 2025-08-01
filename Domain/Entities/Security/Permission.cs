using Domain.Common;
using System;
using System.Collections.Generic;

namespace Domain.Entities.Security
{
    /// <summary>
    /// Represents a system permission that can be checked for authorization
    /// </summary>
    public class Permission : BaseEntity, ISoftDelete
    {
        private Permission() { } // EF Core

        public Permission(string code, string module, string resource, string action, string displayName)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Module = module ?? throw new ArgumentNullException(nameof(module));
            Resource = resource ?? throw new ArgumentNullException(nameof(resource));
            Action = action ?? throw new ArgumentNullException(nameof(action));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Permission code (e.g., "project.view", "budget.approve")
        /// </summary>
        public string Code { get; private set; } = string.Empty;

        /// <summary>
        /// Module this permission belongs to (e.g., "Setup", "Cost", "Progress")
        /// </summary>
        public string Module { get; private set; } = string.Empty;

        /// <summary>
        /// Resource this permission applies to (e.g., "Project", "Budget", "Schedule")
        /// </summary>
        public string Resource { get; private set; } = string.Empty;

        /// <summary>
        /// Action allowed by this permission (e.g., "View", "Create", "Edit", "Delete", "Approve")
        /// </summary>
        public string Action { get; private set; } = string.Empty;

        /// <summary>
        /// User-friendly display name
        /// </summary>
        public string DisplayName { get; private set; } = string.Empty;

        /// <summary>
        /// Detailed description of what this permission allows
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Whether this permission is currently active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Order for display purposes
        /// </summary>
        public int DisplayOrder { get; set; }

        // Navigation properties
        public virtual ICollection<UserProjectPermission> UserProjectPermissions { get; private set; } = new HashSet<UserProjectPermission>();
        public bool IsDeleted { get ; set ; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Methods
        public void UpdateDetails(string displayName, string? description = null)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Checks if this permission matches the given criteria
        /// </summary>
        public bool Matches(string module, string resource, string action)
        {
            return Module.Equals(module, StringComparison.OrdinalIgnoreCase) &&
                   Resource.Equals(resource, StringComparison.OrdinalIgnoreCase) &&
                   Action.Equals(action, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates a permission code from components
        /// </summary>
        public static string CreateCode(string resource, string action)
        {
            return $"{resource.ToLowerInvariant()}.{action.ToLowerInvariant()}";
        }
    }
}