using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities.Configuration.Templates;

/// <summary>
/// Represents a WBS template for project initialization
/// </summary>
public class WBSTemplate : BaseAuditableEntity, ISoftDelete
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string IndustryType { get; private set; } = string.Empty;
    public string ProjectType { get; private set; } = string.Empty;

    // Settings
    public string CodingScheme { get; private set; } = "Numeric";
    public string Delimiter { get; private set; } = ".";
    public int CodeLength { get; private set; } = 3;
    public bool AutoGenerateCodes { get; private set; } = true;

    // Usage
    public bool IsPublic { get; private set; } = true;
    public bool IsActive { get; private set; } = true;
    public int UsageCount { get; private set; }
    public DateTime? LastUsedDate { get; private set; }

    // Navigation properties
    public virtual ICollection<WBSTemplateElement> Elements { get; private set; } =
        new List<WBSTemplateElement>();

    private WBSTemplate() { } // EF Core

    public WBSTemplate(string code, string name, string industryType, string projectType)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IndustryType = industryType ?? throw new ArgumentNullException(nameof(industryType));
        ProjectType = projectType ?? throw new ArgumentNullException(nameof(projectType));
    }

    public void UpdateBasicInfo(
        string name,
        string? description,
        string industryType,
        string projectType
    )
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        IndustryType = industryType ?? throw new ArgumentNullException(nameof(industryType));
        ProjectType = projectType ?? throw new ArgumentNullException(nameof(projectType));
    }

    public void UpdateCodingScheme(
        string codingScheme,
        string delimiter,
        int codeLength,
        bool autoGenerateCodes
    )
    {
        if (codeLength < 1 || codeLength > 10)
            throw new ArgumentException("Code length must be between 1 and 10");

        CodingScheme = codingScheme ?? throw new ArgumentNullException(nameof(codingScheme));
        Delimiter = delimiter ?? throw new ArgumentNullException(nameof(delimiter));
        CodeLength = codeLength;
        AutoGenerateCodes = autoGenerateCodes;
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

    public WBSTemplateElement AddElement(
        string code,
        string name,
        WBSTemplateElement? parent = null,
        string elementType = "Phase",
        bool isOptional = false
    )
    {
        var element = new WBSTemplateElement(code, name, this, parent, elementType, isOptional);
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

    public IEnumerable<WBSTemplateElement> GetRootElements()
    {
        return Elements.Where(e => e.ParentId == null);
    }
}
