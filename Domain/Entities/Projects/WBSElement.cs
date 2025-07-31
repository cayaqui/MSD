using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Common;
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

        ValidateCode();
    }

    // Domain Methods
    public bool IsWorkPackage() => ElementType == WBSElementType.WorkPackage;

    public bool IsPlanningPackage() => ElementType == WBSElementType.PlanningPackage;

    public bool CanHaveChildren() => ElementType != WBSElementType.WorkPackage &&
                                     ElementType != WBSElementType.PlanningPackage;

    public void ConvertToWorkPackage(Guid controlAccountId, ProgressMethod progressMethod)
    {
        if (Children.Any())
            throw new InvalidOperationException("Cannot convert to work package if element has children");

        ElementType = WBSElementType.WorkPackage;
        ControlAccountId = controlAccountId;

        // Create work package details
        WorkPackageDetails = new WorkPackageDetails(Id, progressMethod);

        UpdatedAt = DateTime.UtcNow;
    }

    public void ConvertToPlanningPackage(Guid controlAccountId)
    {
        if (Children.Any())
            throw new InvalidOperationException("Cannot convert to planning package if element has children");

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
        UpdateFullPath();

        return child;
    }

    public void UpdateBasicInfo(string name, string? description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateWBSDictionary(
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

    public void UpdateFullPath()
    {
        var path = new List<string>();
        var current = this;

        while (current != null)
        {
            path.Insert(0, current.Name);
            current = current.Parent;
        }

        FullPath = string.Join("/", path);

        // Update children paths
        foreach (var child in Children)
        {
            child.UpdateFullPath();
        }
    }

    // Calculate rollup values from children
    public void CalculateRollup()
    {
        if (!Children.Any() || IsWorkPackage())
            return;

        var activeChildren = Children.Where(c => c.IsActive && !c.IsDeleted).ToList();
        if (!activeChildren.Any())
            return;

        // Roll up progress (weighted by budget if work packages)
        var workPackages = GetAllWorkPackages().ToList();
        if (workPackages.Any())
        {
            var totalBudget = workPackages.Sum(wp => wp.WorkPackageDetails?.Budget ?? 0);
            if (totalBudget > 0)
            {
                var weightedProgress = workPackages.Sum(wp =>
                    (wp.WorkPackageDetails?.ProgressPercentage ?? 0) *
                    (wp.WorkPackageDetails?.Budget ?? 0));

                // Store rollup progress (might need a property for this)
                var rollupProgress = weightedProgress / totalBudget;
            }
        }

        UpdatedAt = DateTime.UtcNow;
    }

    // Helper methods
    public bool IsLeaf() => !Children.Any();

    public bool CanBeDeleted() => !Children.Any(c => c.IsActive && !c.IsDeleted);

    public int GetDepth()
    {
        if (!Children.Any())
            return 0;

        return Children.Max(c => c.GetDepth()) + 1;
    }

    public IEnumerable<WBSElement> GetAllDescendants()
    {
        foreach (var child in Children)
        {
            yield return child;
            foreach (var descendant in child.GetAllDescendants())
            {
                yield return descendant;
            }
        }
    }

    public IEnumerable<WBSElement> GetAllWorkPackages()
    {
        return GetAllDescendants().Where(w => w.IsWorkPackage());
    }

    public IEnumerable<WBSElement> GetAllPlanningPackages()
    {
        return GetAllDescendants().Where(w => w.IsPlanningPackage());
    }

    private void ValidateCode()
    {
        if (!System.Text.RegularExpressions.Regex.IsMatch(Code, @"^(\d+\.)*\d+$"))
            throw new ArgumentException("WBS code must follow the pattern: 1.2.3 format");
    }

    // Métodos para mapeo CBS
    public void MapToCBS(Guid cbsId, decimal allocationPercentage)
    {
        if (CBSMappings.Any(m => m.CBSId == cbsId))
            throw new InvalidOperationException("CBS already mapped to this WBS element");

        var mapping = new WBSCBSMapping(Id, cbsId, allocationPercentage);
        CBSMappings.Add(mapping);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveCBSMapping(Guid cbsId)
    {
        var mapping = CBSMappings.FirstOrDefault(m => m.CBSId == cbsId);
        if (mapping != null)
        {
            CBSMappings.Remove(mapping);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}