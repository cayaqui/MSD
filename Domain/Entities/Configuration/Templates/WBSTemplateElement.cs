using Domain.Common;
using System;
using System.Collections.Generic;

namespace Domain.Entities.Configuration.Templates;

/// <summary>
/// Represents an element in a WBS template
/// </summary>
public class WBSTemplateElement : BaseEntity, IHierarchical<WBSTemplateElement>
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Template reference
    public Guid WBSTemplateId { get; private set; }
    public virtual WBSTemplate WBSTemplate { get; private set; } = null!;

    // Hierarchy
    public Guid? ParentId { get; private set; }
    public virtual WBSTemplateElement? Parent { get; private set; }
    public virtual ICollection<WBSTemplateElement> Children { get; private set; } =
        new List<WBSTemplateElement>();
    public int Level { get; private set; }
    public int SequenceNumber { get; private set; }
    public string HierarchyPath { get; private set; } = string.Empty;

    // Element configuration
    public string ElementType { get; private set; } = "Phase";
    public bool IsOptional { get; private set; }

    // Default values
    public decimal? DefaultBudgetPercentage { get; private set; }
    public int? DefaultDurationDays { get; private set; }
    public string? DefaultDiscipline { get; private set; }

    private WBSTemplateElement() { } // EF Core

    public WBSTemplateElement(
        string code,
        string name,
        WBSTemplate template,
        WBSTemplateElement? parent = null,
        string elementType = "Phase",
        bool isOptional = false
    )
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        WBSTemplateId = template.Id;
        WBSTemplate = template;
        ElementType = elementType;
        IsOptional = isOptional;

        if (parent != null)
        {
            SetParent(parent);
        }
        else
        {
            Level = 1;
            HierarchyPath = Code;
        }
    }

    public void UpdateBasicInfo(string name, string? description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
    }

    public void SetParent(WBSTemplateElement parent)
    {
        if (parent.WBSTemplateId != WBSTemplateId)
            throw new InvalidOperationException("Parent must belong to the same template");

        if (parent.Id == Id)
            throw new InvalidOperationException("Cannot set self as parent");

        ParentId = parent.Id;
        Parent = parent;
        Level = parent.Level + 1;
        HierarchyPath = string.IsNullOrEmpty(parent.HierarchyPath)
            ? parent.Code
            : $"{parent.HierarchyPath}/{Code}";
    }

    public void SetSequenceNumber(int sequenceNumber)
    {
        if (sequenceNumber < 0)
            throw new ArgumentException("Sequence number cannot be negative");

        SequenceNumber = sequenceNumber;
    }

    public void SetDefaultValues(
        decimal? defaultBudgetPercentage,
        int? defaultDurationDays,
        string? defaultDiscipline
    )
    {
        if (
            defaultBudgetPercentage.HasValue
            && (defaultBudgetPercentage < 0 || defaultBudgetPercentage > 100)
        )
            throw new ArgumentException("Budget percentage must be between 0 and 100");

        if (defaultDurationDays.HasValue && defaultDurationDays < 0)
            throw new ArgumentException("Duration cannot be negative");

        DefaultBudgetPercentage = defaultBudgetPercentage;
        DefaultDurationDays = defaultDurationDays;
        DefaultDiscipline = defaultDiscipline;
    }

    public void SetOptional(bool isOptional)
    {
        IsOptional = isOptional;
    }

    public bool IsDescendantOf(WBSTemplateElement potentialAncestor)
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

    public string GetFullPath(string delimiter = ".")
    {
        if (Parent == null)
            return Code;

        return $"{Parent.GetFullPath(delimiter)}{delimiter}{Code}";
    }
}
