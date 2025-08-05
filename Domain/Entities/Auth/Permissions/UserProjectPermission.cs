using Domain.Common;
using Domain.Entities.Auth.Security;
using Domain.Entities.Organization.Core;
using System;

namespace Domain.Entities.Auth.Permissions
{
    /// <summary>
    /// Represents specific permission overrides for a user on a project
    /// Used for granting or denying permissions beyond their role
    /// </summary>
    public class UserProjectPermission : BaseEntity, IAuditable
    {
        private UserProjectPermission() { } // EF Core

        public UserProjectPermission(Guid userId, Guid projectId, string permissionCode, bool isGranted, string grantedBy)
        {
            UserId = userId;
            ProjectId = projectId;
            PermissionCode = permissionCode ?? throw new ArgumentNullException(nameof(permissionCode));
            IsGranted = isGranted;
            GrantedBy = grantedBy ?? throw new ArgumentNullException(nameof(grantedBy));
            GrantedAt = DateTime.UtcNow;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        // Foreign Keys
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public Guid ProjectId { get; private set; }
        public Project Project { get; private set; } = null!;

        public Guid? PermissionId { get; private set; }
        public Permission? Permission { get; private set; }

        // Permission Details
        /// <summary>
        /// Permission code (e.g., "budget.approve", "schedule.baseline")
        /// Can reference Permission.Code or be a custom permission
        /// </summary>
        public string PermissionCode { get; private set; } = string.Empty;

        /// <summary>
        /// Whether this permission is granted (true) or explicitly denied (false)
        /// Denials override role-based permissions
        /// </summary>
        public bool IsGranted { get; private set; }

        /// <summary>
        /// Optional reason for granting/denying this permission
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// When this permission expires (null = never expires)
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Whether this permission override is currently active
        /// </summary>
        public bool IsActive { get; set; }

        // Audit fields
        public DateTime GrantedAt { get; private set; }
        public string GrantedBy { get; private set; } = string.Empty;
        public DateTime? RevokedAt { get; private set; }
        public string? RevokedBy { get; private set; }

        // Methods
        /// <summary>
        /// Revokes this permission override
        /// </summary>
        public void Revoke(string revokedBy)
        {
            IsActive = false;
            RevokedAt = DateTime.UtcNow;
            RevokedBy = revokedBy ?? throw new ArgumentNullException(nameof(revokedBy));
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Extends the expiration date
        /// </summary>
        public void ExtendExpiration(DateTime newExpirationDate, string extendedBy)
        {
            if (newExpirationDate <= DateTime.UtcNow)
                throw new ArgumentException("Expiration date must be in the future");

            ExpiresAt = newExpirationDate;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = extendedBy;
        }

        /// <summary>
        /// Checks if this permission is currently valid
        /// </summary>
        public bool IsValid()
        {
            return IsActive &&
                   (ExpiresAt == null || ExpiresAt > DateTime.UtcNow) &&
                   RevokedAt == null;
        }

        /// <summary>
        /// Updates the reason for this permission override
        /// </summary>
        public void UpdateReason(string? reason, string updatedBy)
        {
            Reason = reason;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }
    }
}