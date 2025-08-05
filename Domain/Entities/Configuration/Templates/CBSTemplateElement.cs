using Domain.Common;
using System;
using System.Collections.Generic;

namespace Domain.Entities.Configuration.Templates;

/// <summary>
/// Represents an element in a CBS template
/// </summary>
public class CBSTemplateElement : BaseEntity, IHierarchical<CBSTemplateElement>
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    
    // Template reference
    public Guid CBSTemplateId { get; private set; }
    public virtual CBSTemplate CBSTemplate { get; private set; } = null!;
    
    // Hierarchy
    public Guid? ParentId { get; private set; }
    public virtual CBSTemplateElement? Parent { get; private set; }
    public virtual ICollection<CBSTemplateElement> Children { get; private set; } = new List<CBSTemplateElement>();
    public int Level { get; private set; }
    public int SequenceNumber { get; private set; }
    public string HierarchyPath { get; private set; } = string.Empty;


    
    // Cost configuration
    public string CostType { get; private set; } = string.Empty;
    public string? Unit { get; private set; }
    public decimal? UnitRate { get; private set; }
    public bool IsControlAccount { get; private set; }
    
    private CBSTemplateElement() { } // EF Core
    
    public CBSTemplateElement(
        string code,
        string name,
        string costType,
        CBSTemplate template,
        CBSTemplateElement? parent = null,
        bool isControlAccount = false)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        CostType = costType ?? throw new ArgumentNullException(nameof(costType));
        CBSTemplateId = template.Id;
        CBSTemplate = template;
        IsControlAccount = isControlAccount;
        
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
    
    public void UpdateBasicInfo(string name, string? description, string costType)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        CostType = costType ?? throw new ArgumentNullException(nameof(costType));
    }
    
    public void SetParent(CBSTemplateElement parent)
    {
        if (parent.CBSTemplateId != CBSTemplateId)
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
    
    public void SetUnitInfo(string? unit, decimal? unitRate)
    {
        if (unitRate.HasValue && unitRate < 0)
            throw new ArgumentException("Unit rate cannot be negative");
        
        Unit = unit;
        UnitRate = unitRate;
    }
    
    public void SetAsControlAccount(bool isControlAccount)
    {
        IsControlAccount = isControlAccount;
    }
    
    public bool IsDescendantOf(CBSTemplateElement potentialAncestor)
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