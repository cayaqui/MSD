using Domain.Common;
using Domain.Entities.Auth.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities.Organization.Core
{
    /// <summary>
    /// Organizational Breakdown Structure (OBS) node
    /// Represents organizational hierarchy for project resource allocation
    /// </summary>
    public class OBSNode : BaseEntity, IHierarchical<OBSNode>
    {
        public string Code { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string NodeType { get; private set; } = "Department"; // Company, Division, Department, Team, Role
        
        // Hierarchical structure
        public Guid? ParentId { get; private set; }
        public virtual OBSNode? Parent { get; private set; }
        public virtual ICollection<OBSNode> Children { get; private set; } = new List<OBSNode>();
        public int Level { get; private set; }
        public string HierarchyPath { get; private set; } = string.Empty; // e.g., "001.002.003"
        
        // Project association
        public Guid? ProjectId { get; private set; }
        public virtual Project? Project { get; private set; }
        
        // Manager/responsible person
        public Guid? ManagerId { get; private set; }
        public virtual User? Manager { get; private set; }
        
        // Cost center for accounting
        public string? CostCenter { get; private set; }
        
        // Resource capacity
        public decimal? TotalFTE { get; private set; } // Full-time equivalents
        public decimal? AvailableFTE { get; private set; }
        
        // Navigation properties
        public virtual ICollection<User> Members { get; private set; } = new List<User>();
        
        // Status
        public bool IsActive { get; set; } = true;
        
        private OBSNode() { } // EF Core constructor

        public OBSNode(string code, string name, string nodeType, int level)
        {
            Code = code;
            Name = name;
            NodeType = nodeType;
            Level = level;
            HierarchyPath = code;
        }

        public void SetParent(OBSNode parent)
        {
            if (parent.Id == Id)
                throw new InvalidOperationException("Cannot set self as parent");
            
            ParentId = parent.Id;
            Parent = parent;
            Level = parent.Level + 1;
            UpdateHierarchyPath();
        }

        public void RemoveParent()
        {
            ParentId = null;
            Parent = null;
            Level = 0;
            UpdateHierarchyPath();
        }

        public void UpdateHierarchyPath()
        {
            if (Parent != null)
            {
                HierarchyPath = $"{Parent.HierarchyPath}.{Code}";
            }
            else
            {
                HierarchyPath = Code;
            }
            
            // Update children paths recursively
            foreach (var child in Children)
            {
                child.UpdateHierarchyPath();
            }
        }

        public void AssignToProject(Guid projectId)
        {
            ProjectId = projectId;
        }

        public void SetManager(Guid managerId)
        {
            ManagerId = managerId;
        }

        public void SetCostCenter(string costCenter)
        {
            CostCenter = costCenter;
        }

        public void UpdateBasicInfo(string name, string? description, string nodeType)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            NodeType = nodeType ?? throw new ArgumentNullException(nameof(nodeType));
        }

        public void UpdateCapacity(decimal totalFTE, decimal availableFTE)
        {
            if (totalFTE < 0 || availableFTE < 0)
                throw new ArgumentException("FTE values cannot be negative");
            
            if (availableFTE > totalFTE)
                throw new ArgumentException("Available FTE cannot exceed total FTE");
            
            TotalFTE = totalFTE;
            AvailableFTE = availableFTE;
        }

        public void AddMember(User user)
        {
            if (!Members.Any(m => m.Id == user.Id))
            {
                Members.Add(user);
            }
        }

        public void RemoveMember(User user)
        {
            Members.Remove(user);
        }

        public decimal GetUtilizationRate()
        {
            if (TotalFTE == null || TotalFTE == 0)
                return 0;
            
            var allocated = TotalFTE.Value - (AvailableFTE ?? 0);
            return allocated / TotalFTE.Value * 100;
        }

        public bool IsDescendantOf(OBSNode potentialAncestor)
        {
            var current = Parent;
            while (current != null)
            {
                if (current.Id == potentialAncestor.Id)
                    return true;
                current = current.Parent;
            }
            return false;
        }

        public static class NodeTypes
        {
            public const string Company = "Company";
            public const string Division = "Division";
            public const string Department = "Department";
            public const string Team = "Team";
            public const string Role = "Role";
        }
    }
}