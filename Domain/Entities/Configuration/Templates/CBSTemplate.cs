using Domain.Common;
using Core.Enums.Cost;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities.Configuration.Templates;

/// <summary>
/// Represents a CBS (Cost Breakdown Structure) template for project initialization
/// </summary>
public class CBSTemplate : BaseAuditableEntity, ISoftDelete
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string IndustryType { get; private set; } = string.Empty;
    public CostType CostType { get; private set; }

    // Settings
    public string CodingScheme { get; private set; } = "Numeric";
    public string Delimiter { get; private set; } = ".";
    public bool IncludesIndirectCosts { get; private set; } = true;
    public bool IncludesContingency { get; private set; } = true;

    // Usage
    public bool IsPublic { get; private set; } = true;
    public bool IsActive { get; private set; } = true;
    public int UsageCount { get; private set; }
    public DateTime? LastUsedDate { get; private set; }

    // Navigation properties
    public virtual ICollection<CBSTemplateElement> Elements { get; private set; } =
        new List<CBSTemplateElement>();

    private CBSTemplate() { } // EF Core

    public CBSTemplate(string code, string name, string industryType, CostType costType)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IndustryType = industryType ?? throw new ArgumentNullException(nameof(industryType));
        CostType = costType;
    }

    public void UpdateBasicInfo(
        string name,
        string? description,
        string industryType,
        CostType? costType
    )
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        IndustryType = industryType ?? throw new ArgumentNullException(nameof(industryType));
        CostType = costType ?? throw new ArgumentNullException(nameof(costType));
    }

    public void UpdateSettings(
        string codingScheme,
        string delimiter,
        bool includesIndirectCosts,
        bool includesContingency
    )
    {
        CodingScheme = codingScheme ?? throw new ArgumentNullException(nameof(codingScheme));
        Delimiter = delimiter ?? throw new ArgumentNullException(nameof(delimiter));
        IncludesIndirectCosts = includesIndirectCosts;
        IncludesContingency = includesContingency;
    }

    public void SetVisibility(bool isPublic)
    {
        IsPublic = isPublic;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void IncrementUsage()
    {
        UsageCount++;
        LastUsedDate = DateTime.UtcNow;
    }

    public CBSTemplateElement AddElement(
        string code,
        string name,
        string costType,
        CBSTemplateElement? parent = null,
        bool isControlAccount = false
    )
    {
        var element = new CBSTemplateElement(code, name, costType, this, parent, isControlAccount);
        Elements.Add(element);
        return element;
    }

    public void RemoveElement(Guid elementId)
    {
        var element = Elements.FirstOrDefault(e => e.Id == elementId);
        if (element != null)
        {
            Elements.Remove(element);
        }
    }

    public int GetMaxLevel()
    {
        if (!Elements.Any())
            return 0;

        return Elements.Max(e => e.Level);
    }

    public IEnumerable<CBSTemplateElement> GetRootElements()
    {
        return Elements.Where(e => e.ParentId == null);
    }

    public IEnumerable<CBSTemplateElement> GetControlAccounts()
    {
        return Elements.Where(e => e.IsControlAccount);
    }
}
