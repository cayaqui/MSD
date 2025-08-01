using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Common;
using Domain.Entities.Cost;
using Core.Enums.Projects;
using Core.Enums.Progress;

namespace Domain.Entities.Projects;

/// <summary>
/// Work Breakdown Structure Element - Representa cualquier nivel de la WBS
/// incluyendo Work Packages en el nivel más bajo
/// </summary>
public class WBSElement : BaseEntity, IAuditable, ISoftDelete, IActivatable
{
    // Identificación
    public string Code { get; private set; } = string.Empty; // e.g., "1.2.3.4"
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Jerarquía
    public Guid ProjectId { get; private set; }
    public Guid? ParentId { get; private set; }
    public int Level { get; private set; }
    public int SequenceNumber { get; private set; }
    public string FullPath { get; private set; } = string.Empty; // e.g., "Project/Phase/Deliverable/WorkPackage"

    // Clasificación
    public WBSElementType ElementType { get; private set; }

    // Control Account (solo para Work Packages y Planning Packages)
    public Guid? ControlAccountId { get; private set; }

    // Detalles específicos de Work Package
    public WorkPackageDetails? WorkPackageDetails { get; private set; }

    // WBS Dictionary
    public string? DeliverableDescription { get; private set; }
    public string? AcceptanceCriteria { get; private set; }
    public string? Assumptions { get; private set; }
    public string? Constraints { get; private set; }
    public string? ExclusionsInclusions { get; private set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Activatable
    public bool IsActive { get; set; }

    // Navigation Properties
    public Project Project { get; private set; } = null!;
    public WBSElement? Parent { get; private set; }
    public ICollection<WBSElement> Children { get; private set; } = new List<WBSElement>();
    public ControlAccount? ControlAccount { get; private set; }
    public ICollection<WBSCBSMapping> CBSMappings { get; private set; } = new List<WBSCBSMapping>();

    // Constructor for EF Core
    private WBSElement() { }

    public WBSElement(
        Guid projectId,
        string code,
        string name,
        WBSElementType elementType,
        int level,
        int sequenceNumber,
        Guid? parentId = null)
    {
        ProjectId = projectId;
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ElementType = elementType;
        Level = level;
        SequenceNumber = sequenceNumber;
        ParentId = parentId;

        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        FullPath = name; // Will be updated when parent is set

        ValidateCode();
        ValidateElementType();
    }

    // Domain Methods
    public bool IsWorkPackage() => ElementType == WBSElementType.WorkPackage;

    public bool IsPlanningPackage() => ElementType == WBSElementType.PlanningPackage;

    public bool CanHaveChildren() => ElementType != WBSElementType.WorkPackage &&
                                     ElementType != WBSElementType.PlanningPackage;

    public void ConvertToWorkPackage(Guid controlAccountId, ProgressMethod progressMethod, )
    {
        if (Children.Any())
            throw new InvalidOperationException("Cannot convert to work package if element has children");

        if (ElementType == WBSElementType.WorkPackage)
            throw new InvalidOperationException("Element is already a work package");

        ElementType = WBSElementType.WorkPackage;
        ControlAccountId = controlAccountId;

        // Create work package details
        WorkPackageDetails = new WorkPackageDetails(Id, GetTotalBudget(), progressMethod);

        UpdatedAt = DateTime.UtcNow;
    }

    public void ConvertToPlanningPackage(Guid controlAccountId)
    {
        if (Children.Any())
            throw new InvalidOperationException("Cannot convert to planning package if element has children");

        if (ElementType == WBSElementType.PlanningPackage)
            throw new InvalidOperationException("Element is already a planning package");

        ElementType = WBSElementType.PlanningPackage;
        ControlAccountId = controlAccountId;

        UpdatedAt = DateTime.UtcNow;
    }

    public void ConvertPlanningPackageToWorkPackage(ProgressMethod progressMethod)
    {
        if (ElementType != WBSElementType.PlanningPackage)
            throw new InvalidOperationException("Only planning packages can be converted to work packages");

        ElementType = WBSElementType.WorkPackage;
        WorkPackageDetails = new WorkPackageDetails(Id, progressMethod);

        UpdatedAt = DateTime.UtcNow;
    }

    public WBSElement AddChild(string code, string name, WBSElementType elementType, int sequenceNumber)
    {
        if (!CanHaveChildren())
            throw new InvalidOperationException($"{ElementType} cannot have children");

        var child = new WBSElement(
            ProjectId,
            code,
            name,
            elementType,
            Level + 1,
            sequenceNumber,
            Id);

        Children.Add(child);
        child.Parent = this;
        child.UpdateFullPath();

        return child;
    }

    public void UpdateBasicInfo(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        Name = name;
        Description = description;
        UpdateFullPath();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCode(string newCode)
    {
        if (string.IsNullOrWhiteSpace(newCode))
            throw new ArgumentNullException(nameof(newCode));

        Code = newCode;
        ValidateCode();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSequenceNumber(int newSequenceNumber)
    {
        if (newSequenceNumber < 1)
            throw new ArgumentException("Sequence number must be greater than 0");

        SequenceNumber = newSequenceNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDictionaryInfo(
        string? deliverableDescription,
        string? acceptanceCriteria,
        string? assumptions,
        string? constraints,
        string? exclusionsInclusions)
    {
        DeliverableDescription = deliverableDescription;
        AcceptanceCriteria = acceptanceCriteria;
        Assumptions = assumptions;
        Constraints = constraints;
        ExclusionsInclusions = exclusionsInclusions;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToControlAccount(Guid controlAccountId)
    {
        if (ElementType != WBSElementType.WorkPackage && ElementType != WBSElementType.PlanningPackage)
            throw new InvalidOperationException("Only work packages and planning packages can be assigned to control accounts");

        ControlAccountId = controlAccountId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveFromControlAccount()
    {
        ControlAccountId = null;
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

    public void Delete(string deletedBy)
    {
        if (string.IsNullOrWhiteSpace(deletedBy))
            throw new ArgumentNullException(nameof(deletedBy));

        if (Children.Any(c => !c.IsDeleted))
            throw new InvalidOperationException("Cannot delete WBS element with active children");

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
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    // CBS Mapping Methods
    public void AddCBSMapping(Guid cbsId, decimal allocationPercentage, bool isPrimary = false)
    {
        // Validate total allocation doesn't exceed 100%
        var currentTotal = CBSMappings
            .Where(m => m.IsActive())
            .Sum(m => m.AllocationPercentage);

        if (currentTotal + allocationPercentage > 100)
            throw new InvalidOperationException("Total CBS allocation cannot exceed 100%");

        // If setting as primary, remove primary flag from others
        if (isPrimary)
        {
            foreach (var mapping in CBSMappings.Where(m => m.IsPrimary))
            {
                mapping.SetAsPrimary();
            }
        }

        var newMapping = new WBSCBSMapping(Id, cbsId, allocationPercentage, isPrimary);
        CBSMappings.Add(newMapping);
    }

    public void RemoveCBSMapping(Guid cbsId)
    {
        var mapping = CBSMappings.FirstOrDefault(m => m.CBSId == cbsId && m.IsActive());
        if (mapping != null)
        {
            mapping.SetEndDate(DateTime.UtcNow);
        }
    }

    // Private Methods
    private void UpdateFullPath()
    {
        var pathParts = new List<string> { Name };
        var current = Parent;

        while (current != null)
        {
            pathParts.Insert(0, current.Name);
            current = current.Parent;
        }

        FullPath = string.Join("/", pathParts);
    }

    private void ValidateCode()
    {
        if (string.IsNullOrWhiteSpace(Code))
            throw new ArgumentException("WBS code cannot be empty");

        // Validate format (e.g., 1.2.3)
        var parts = Code.Split('.');
        foreach (var part in parts)
        {
            if (!int.TryParse(part, out _))
                throw new ArgumentException("WBS code must be in format X.Y.Z (e.g., 1.2.3)");
        }
    }

    private void ValidateElementType()
    {
        // Root elements cannot be Work Packages or Planning Packages
        if (!ParentId.HasValue &&
            (ElementType == WBSElementType.WorkPackage ||
             ElementType == WBSElementType.PlanningPackage))
        {
            throw new InvalidOperationException("Root elements cannot be Work Packages or Planning Packages");
        }
    }

    // Helper Methods for Queries
    public bool HasActiveChildren() => Children.Any(c => !c.IsDeleted && c.IsActive);

    public int GetTotalDescendantsCount()
    {
        var count = Children.Count;
        foreach (var child in Children)
        {
            count += child.GetTotalDescendantsCount();
        }
        return count;
    }

    public List<WBSElement> GetAllDescendants()
    {
        var descendants = new List<WBSElement>();
        GetDescendantsRecursive(this, descendants);
        return descendants;
    }

    private void GetDescendantsRecursive(WBSElement element, List<WBSElement> descendants)
    {
        foreach (var child in element.Children)
        {
            descendants.Add(child);
            GetDescendantsRecursive(child, descendants);
        }
    }

    public decimal GetTotalBudget()
    {
        decimal total = 0;

        if (WorkPackageDetails != null)
        {
            total = WorkPackageDetails.Budget;
        }
        else
        {
            foreach (var child in Children.Where(c => !c.IsDeleted))
            {
                total += child.GetTotalBudget();
            }
        }

        return total;
    }

    public decimal GetWeightedProgress()
    {
        if (WorkPackageDetails != null)
        {
            return WorkPackageDetails.ProgressPercentage;
        }

        if (!Children.Any(c => !c.IsDeleted))
        {
            return 0;
        }

        decimal totalWeight = 0;
        decimal weightedProgress = 0;

        foreach (var child in Children.Where(c => !c.IsDeleted))
        {
            var childBudget = child.GetTotalBudget();
            totalWeight += childBudget;
            weightedProgress += childBudget * child.GetWeightedProgress();
        }

        return totalWeight > 0 ? weightedProgress / totalWeight : 0;
    }
}