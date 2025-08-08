using Domain.Entities.Cost.Control;
using Domain.Entities.WBS;
using System;
using System.Linq;

namespace Domain.Entities.Organization.Core
{
    /// <summary>
    /// Responsibility Assignment Matrix (RAM) - RACI Matrix
    /// Maps OBS to WBS to define roles and responsibilities
    /// </summary>
    public class RAM : BaseEntity
    {
        public Guid ProjectId { get; private set; }
        public virtual Project Project { get; private set; } = null!;
        
        // WBS Element being assigned
        public Guid WBSElementId { get; private set; }
        public virtual WBSElement WBSElement { get; private set; } = null!;
        
        // OBS Node (organizational unit/role)
        public Guid OBSNodeId { get; private set; }
        public virtual OBSNode OBSNode { get; private set; } = null!;
        
        // RACI assignment
        public string ResponsibilityType { get; private set; } = string.Empty; // R, A, C, I
        
        // Additional details
        public decimal? AllocatedHours { get; private set; }
        public decimal? AllocatedPercentage { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        
        // Control Account linkage (if applicable)
        public Guid? ControlAccountId { get; private set; }
        public virtual ControlAccount? ControlAccount { get; private set; }
        
        public string? Notes { get; private set; }
        public bool IsActive { get; set; } = true;
        
        private RAM() { } // EF Core constructor

        public RAM(
            Guid projectId,
            Guid wbsElementId,
            Guid obsNodeId,
            string responsibilityType)
        {
            ProjectId = projectId;
            WBSElementId = wbsElementId;
            OBSNodeId = obsNodeId;
            SetResponsibilityType(responsibilityType);
        }

        private void SetResponsibilityType(string type)
        {
            if (!ResponsibilityTypes.IsValid(type))
                throw new ArgumentException($"Invalid responsibility type: {type}");
            
            ResponsibilityType = type.ToUpper();
        }

        public void UpdateAllocation(decimal? hours, decimal? percentage)
        {
            if (hours.HasValue && hours.Value < 0)
                throw new ArgumentException("Allocated hours cannot be negative");
            
            if (percentage.HasValue && (percentage.Value < 0 || percentage.Value > 100))
                throw new ArgumentException("Allocated percentage must be between 0 and 100");
            
            AllocatedHours = hours;
            AllocatedPercentage = percentage;
        }

        public void SetPeriod(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date must be after start date");
            
            StartDate = startDate;
            EndDate = endDate;
        }

        public void LinkToControlAccount(Guid controlAccountId)
        {
            ControlAccountId = controlAccountId;
        }

        public void AddNotes(string notes)
        {
            Notes = notes;
        }

        public bool IsResponsible() => ResponsibilityType == ResponsibilityTypes.Responsible;
        public bool IsAccountable() => ResponsibilityType == ResponsibilityTypes.Accountable;
        public bool IsConsulted() => ResponsibilityType == ResponsibilityTypes.Consulted;
        public bool IsInformed() => ResponsibilityType == ResponsibilityTypes.Informed;

        public static class ResponsibilityTypes
        {
            public const string Responsible = "R"; // Does the work
            public const string Accountable = "A"; // Ultimately answerable
            public const string Consulted = "C"; // Provides input
            public const string Informed = "I"; // Kept in the loop
            public const string Support = "S"; // Provides resources (RASCI variant)
            public const string Verifier = "V"; // Checks the work (RACI-V variant)

            public static bool IsValid(string type)
            {
                return new[] { Responsible, Accountable, Consulted, Informed, Support, Verifier }
                    .Contains(type?.ToUpper());
            }

            public static string GetDescription(string type)
            {
                return type?.ToUpper() switch
                {
                    Responsible => "Responsible - Does the work to complete the task",
                    Accountable => "Accountable - Ultimately answerable for correct completion",
                    Consulted => "Consulted - Provides input before the work",
                    Informed => "Informed - Kept up-to-date on progress",
                    Support => "Support - Provides resources or support",
                    Verifier => "Verifier - Checks the work meets criteria",
                    _ => "Unknown"
                };
            }
        }
    }
}