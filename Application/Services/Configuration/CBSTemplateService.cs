using Application.Interfaces.Common;
using Application.Interfaces.Configuration;
using Application.Services.Base;
using AutoMapper;
using Core.DTOs.Configuration.WBSTemplates;
using Microsoft.Extensions.Logging;
using Core.Enums.Cost;
using Domain.Entities.Configuration.Templates;
using Domain.Entities.Organization.Core;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Configuration;

/// <summary>
/// Service implementation for managing CBS templates
/// </summary>
public class CBSTemplateService : BaseService<CBSTemplate, CBSTemplateDto, CreateCBSTemplateDto, UpdateCBSTemplateDto>, ICBSTemplateService
{
    public CBSTemplateService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CBSTemplateService> logger) 
        : base(unitOfWork, mapper, logger)
    {
    }

    public async Task<CBSTemplateDto?> GetByCodeAsync(string code)
    {
        var template = await _unitOfWork.Repository<CBSTemplate>()
            .GetAsync(
                filter: t => t.Code == code && !t.IsDeleted,
                includeProperties: "Elements");
        
        if (template == null)
            return null;

        var dto = _mapper.Map<CBSTemplateDto>(template);
        dto.TotalElements = template.Elements.Count;
        dto.MaxLevel = template.GetMaxLevel();
        
        return dto;
    }

    public async Task<IEnumerable<CBSTemplateDto>> GetActiveAsync()
    {
        var templates = await _unitOfWork.Repository<CBSTemplate>()
            .GetAllAsync(
                filter: t => t.IsActive && !t.IsDeleted,
                orderBy: q => q.OrderBy(t => t.Name),
                includeProperties: "Elements");
        
        var dtos = _mapper.Map<IEnumerable<CBSTemplateDto>>(templates);
        
        foreach (var dto in dtos)
        {
            var template = templates.FirstOrDefault(t => t.Id == dto.Id);
            if (template != null)
            {
                dto.TotalElements = template.Elements.Count;
                dto.MaxLevel = template.GetMaxLevel();
            }
        }
        
        return dtos;
    }

    public async Task<IEnumerable<CBSTemplateDto>> GetByIndustryTypeAsync(string industryType)
    {
        var templates = await _unitOfWork.Repository<CBSTemplate>()
            .GetAllAsync(
                filter: t => t.IndustryType == industryType && t.IsActive && !t.IsDeleted,
                orderBy: q => q.OrderBy(t => t.Name));
        
        return _mapper.Map<IEnumerable<CBSTemplateDto>>(templates);
    }

    public async Task<IEnumerable<CBSTemplateDto>> GetByCostTypeAsync(string costType)
    {
        if (!Enum.TryParse<CostType>(costType, out var costTypeEnum))
            return new List<CBSTemplateDto>();

        var templates = await _unitOfWork.Repository<CBSTemplate>()
            .GetAllAsync(
                filter: t => t.CostType == costTypeEnum && t.IsActive && !t.IsDeleted,
                orderBy: q => q.OrderBy(t => t.Name));
        
        return _mapper.Map<IEnumerable<CBSTemplateDto>>(templates);
    }

    public async Task<IEnumerable<CBSTemplateDto>> SearchAsync(WBSCBSTemplateFilterDto filter)
    {
        var query = _unitOfWork.Repository<CBSTemplate>()
            .Query()
            .Where(t => !t.IsDeleted);

        if (filter.Type == "CBS")
        {
            if (!string.IsNullOrWhiteSpace(filter.IndustryType))
                query = query.Where(t => t.IndustryType == filter.IndustryType);

            if (filter.IsPublic.HasValue)
                query = query.Where(t => t.IsPublic == filter.IsPublic.Value);

            if (filter.IsActive.HasValue)
                query = query.Where(t => t.IsActive == filter.IsActive.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(t => 
                    t.Code.ToLower().Contains(searchTerm) ||
                    t.Name.ToLower().Contains(searchTerm) ||
                    (t.Description != null && t.Description.ToLower().Contains(searchTerm)));
            }
        }

        var templates = await query
            .OrderBy(t => t.Name)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CBSTemplateDto>>(templates);
    }

    public async Task<IEnumerable<CBSTemplateElementDto>> GetElementsAsync(Guid templateId)
    {
        var elements = await _unitOfWork.Repository<CBSTemplateElement>()
            .GetAllAsync(
                filter: e => e.CBSTemplateId == templateId,
                orderBy: q => q.OrderBy(e => e.Level).ThenBy(e => e.SequenceNumber),
                includeProperties: "Children");
        
        return _mapper.Map<IEnumerable<CBSTemplateElementDto>>(elements);
    }

    public async Task<CBSTemplateElementDto?> AddElementAsync(Guid templateId, CBSTemplateElementDto element)
    {
        var template = await _unitOfWork.Repository<CBSTemplate>()
            .GetAsync(filter: t => t.Id == templateId && !t.IsDeleted);

        if (template == null)
            return null;

        CBSTemplateElement? parent = null;
        if (element.ParentId.HasValue)
        {
            parent = await _unitOfWork.Repository<CBSTemplateElement>()
                .GetAsync(filter: e => e.Id == element.ParentId.Value);
        }

        var newElement = template.AddElement(
            element.Code,
            element.Name,
            element.CostType,
            parent,
            element.IsControlAccount);

        if (!string.IsNullOrWhiteSpace(element.Description))
            newElement.UpdateBasicInfo(element.Name, element.Description, element.CostType);

        if (!string.IsNullOrWhiteSpace(element.Unit) || element.UnitRate.HasValue)
            newElement.SetUnitInfo(element.Unit, element.UnitRate);

        _unitOfWork.Repository<CBSTemplate>().Update(template);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CBSTemplateElementDto>(newElement);
    }

    public async Task<bool> UpdateElementAsync(Guid elementId, CBSTemplateElementDto element)
    {
        var existingElement = await _unitOfWork.Repository<CBSTemplateElement>()
            .GetAsync(filter: e => e.Id == elementId);

        if (existingElement == null)
            return false;

        existingElement.UpdateBasicInfo(element.Name, element.Description, element.CostType);
        existingElement.SetUnitInfo(element.Unit, element.UnitRate);
        existingElement.SetAsControlAccount(element.IsControlAccount);

        _unitOfWork.Repository<CBSTemplateElement>().Update(existingElement);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveElementAsync(Guid templateId, Guid elementId)
    {
        var template = await _unitOfWork.Repository<CBSTemplate>()
            .GetAsync(
                filter: t => t.Id == templateId && !t.IsDeleted,
                includeProperties: "Elements");

        if (template == null)
            return false;

        template.RemoveElement(elementId);
        
        _unitOfWork.Repository<CBSTemplate>().Update(template);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<CBSTemplateElementDto>> GetControlAccountsAsync(Guid templateId)
    {
        var elements = await _unitOfWork.Repository<CBSTemplateElement>()
            .GetAllAsync(
                filter: e => e.CBSTemplateId == templateId && e.IsControlAccount,
                orderBy: q => q.OrderBy(e => e.Code));
        
        return _mapper.Map<IEnumerable<CBSTemplateElementDto>>(elements);
    }

    public async Task<int> ApplyToProjectAsync(Guid templateId, Guid projectId)
    {
        var template = await _unitOfWork.Repository<CBSTemplate>()
            .GetAsync(
                filter: t => t.Id == templateId && !t.IsDeleted,
                includeProperties: "Elements");

        if (template == null)
            return 0;

        var project = await _unitOfWork.Repository<Project>()
            .GetAsync(filter: p => p.Id == projectId && !p.IsDeleted);

        if (project == null)
            return 0;

        var createdCount = 0;
        
        // Create CBS elements in the project based on template elements
        // Implementation would depend on your CBS entity structure
        
        template.IncrementUsage();
        _unitOfWork.Repository<CBSTemplate>().Update(template);
        
        await _unitOfWork.SaveChangesAsync();
        
        return createdCount;
    }

    public async Task<CBSTemplateDto?> CloneAsync(Guid sourceId, string newCode, string newName)
    {
        var source = await _unitOfWork.Repository<CBSTemplate>()
            .GetAsync(
                filter: t => t.Id == sourceId && !t.IsDeleted,
                includeProperties: "Elements");

        if (source == null)
            return null;

        var clone = new CBSTemplate(newCode, newName, source.IndustryType, source.CostType);
        
        clone.UpdateBasicInfo(newName, $"Cloned from {source.Name}", source.IndustryType, source.CostType);
        clone.UpdateSettings(source.CodingScheme, source.Delimiter, source.IncludesIndirectCosts, source.IncludesContingency);
        
        await _unitOfWork.Repository<CBSTemplate>().AddAsync(clone);
        
        // Clone elements
        var elementMapping = new Dictionary<Guid, CBSTemplateElement>();
        
        foreach (var sourceElement in source.Elements.Where(e => e.ParentId == null))
        {
            await CloneElementRecursive(sourceElement, clone, null, elementMapping);
        }
        
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<CBSTemplateDto>(clone);
    }

    private async Task CloneElementRecursive(
        CBSTemplateElement sourceElement, 
        CBSTemplate targetTemplate, 
        CBSTemplateElement? targetParent,
        Dictionary<Guid, CBSTemplateElement> elementMapping)
    {
        var clonedElement = targetTemplate.AddElement(
            sourceElement.Code,
            sourceElement.Name,
            sourceElement.CostType,
            targetParent,
            sourceElement.IsControlAccount);

        if (!string.IsNullOrWhiteSpace(sourceElement.Description))
            clonedElement.UpdateBasicInfo(sourceElement.Name, sourceElement.Description, sourceElement.CostType);

        clonedElement.SetUnitInfo(sourceElement.Unit, sourceElement.UnitRate);

        elementMapping[sourceElement.Id] = clonedElement;

        // Clone children
        foreach (var child in sourceElement.Children)
        {
            await CloneElementRecursive(child, targetTemplate, clonedElement, elementMapping);
        }
    }

    public async Task<bool> ActivateAsync(Guid id)
    {
        var template = await _unitOfWork.Repository<CBSTemplate>()
            .GetAsync(filter: t => t.Id == id && !t.IsDeleted);

        if (template == null)
            return false;

        template.Activate();
        
        _unitOfWork.Repository<CBSTemplate>().Update(template);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var template = await _unitOfWork.Repository<CBSTemplate>()
            .GetAsync(filter: t => t.Id == id && !t.IsDeleted);

        if (template == null)
            return false;

        template.Deactivate();
        
        _unitOfWork.Repository<CBSTemplate>().Update(template);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<CBSTemplateDto?> ImportAsync(byte[] data, string format = "Excel")
    {
        // This would import template from Excel/XML/JSON format
        // Implementation would depend on the import library being used
        throw new NotImplementedException("Import functionality to be implemented");
    }

    public async Task<byte[]> ExportAsync(Guid templateId, string format = "Excel")
    {
        // This would export template to specified format
        // Implementation would depend on the export library being used
        throw new NotImplementedException("Export functionality to be implemented");
    }

    public async Task<TemplateValidationResult> ValidateStructureAsync(Guid templateId)
    {
        var result = new TemplateValidationResult { IsValid = true };
        
        var template = await _unitOfWork.Repository<CBSTemplate>()
            .GetAsync(
                filter: t => t.Id == templateId && !t.IsDeleted,
                includeProperties: "Elements");

        if (template == null)
        {
            result.IsValid = false;
            result.Errors.Add("Template not found");
            return result;
        }

        var elements = template.Elements.ToList();
        result.ElementsValidated = elements.Count;

        // Validate hierarchy
        foreach (var element in elements)
        {
            // Check for circular references
            if (element.ParentId.HasValue)
            {
                var parent = elements.FirstOrDefault(e => e.Id == element.ParentId.Value);
                if (parent == null)
                {
                    result.Errors.Add($"Element {element.Code} references non-existent parent");
                    result.IsValid = false;
                    result.ErrorCount++;
                }
                else if (parent.IsDescendantOf(element))
                {
                    result.Errors.Add($"Circular reference detected for element {element.Code}");
                    result.IsValid = false;
                    result.ErrorCount++;
                }
            }

            // Check for duplicate codes
            var duplicates = elements.Where(e => e.Code == element.Code && e.Id != element.Id).ToList();
            if (duplicates.Any())
            {
                result.Warnings.Add($"Duplicate code found: {element.Code}");
                result.WarningCount++;
            }

            // Check control account hierarchy
            if (element.IsControlAccount && element.Children.Any(c => c.IsControlAccount))
            {
                result.Warnings.Add($"Control account {element.Code} has child control accounts");
                result.WarningCount++;
            }
        }

        return result;
    }

    public async Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null)
    {
        var query = _unitOfWork.Repository<CBSTemplate>()
            .Query()
            .Where(t => t.Code == code && !t.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId.Value);

        return !await query.AnyAsync();
    }

    public async Task<TemplateUsageStatisticsDto> GetUsageStatisticsAsync(Guid templateId)
    {
        var template = await _unitOfWork.Repository<CBSTemplate>()
            .GetAsync(filter: t => t.Id == templateId && !t.IsDeleted);

        if (template == null)
            throw new InvalidOperationException("Template not found");

        // This would require tracking template usage in projects
        // For now, returning basic statistics
        return new TemplateUsageStatisticsDto
        {
            TemplateId = templateId,
            TemplateName = template.Name,
            TemplateType = "CBS",
            TotalUsageCount = template.UsageCount,
            LastUsedDate = template.LastUsedDate,
            // Other statistics would be calculated from project data
        };
    }

    public async Task<int> MapToWBSAsync(Guid templateId, Guid projectId, Dictionary<Guid, List<Guid>> cbsToWbsMapping)
    {
        // This would map CBS elements to WBS elements
        // Implementation would depend on your mapping logic
        throw new NotImplementedException("CBS to WBS mapping functionality to be implemented");
    }
}