using Domain.Common;
using Domain.Entities.Organization.Core;
using System;

namespace Domain.Entities.Auth.Security
{
    public class ProjectTeamMember : BaseEntity, IAuditable
    {
        // Foreign Keys
        public Guid ProjectId { get; private set; }
        public Project Project { get; private set; } = null!;
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        // Team Member Details
        public string Role { get; set; } = string.Empty;
        public decimal? AllocationPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }

        private ProjectTeamMember() { } // EF Core

        public ProjectTeamMember(Guid projectId, Guid userId, string role, DateTime startDate)
        {
            ProjectId = projectId;
            UserId = userId;
            Role = role ?? throw new ArgumentNullException(nameof(role));
            StartDate = startDate;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
