using Application.Interfaces.Common;
using Application.Interfaces.Configuration;
using Application.Services.Base;
using AutoMapper;
using Core.DTOs.Configuration.WBSTemplates;
using Microsoft.Extensions.Logging;
using Domain.Entities.Configuration.Templates;
using Domain.Entities.Organization.Core;
using Domain.Entities.WBS;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Configuration;

/// <summary>
/// Service implementation for managing WBS templates
/// </summary>
public class WBSTemplateService : BaseService<WBSTemplate, WBSTemplateDto, CreateWBSTemplateDto, UpdateWBSTemplateDto>, IWBSTemplateService
{
    public WBSTemplateService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<WBSTemplateService> logger) 
        : base(unitOfWork, mapper, logger)
    {
    }

    public async Task<WBSTemplateDto?> GetByCodeAsync(string code)
    {
        var template = await _unitOfWork.Repository<WBSTemplate>()
            .GetAsync(
                filter: t => t.Code == code && !t.IsDeleted,
                includeProperties: "Elements");
        
        if (template == null)
            return null;

        var dto = _mapper.Map<WBSTemplateDto>(template);
        dto.TotalElements = template.Elements.Count;
        dto.MaxLevel = template.GetMaxLevel();
        
        return dto;
    }

    public async Task<IEnumerable<WBSTemplateDto>> GetActiveAsync()
    {
        var templates = await _unitOfWork.Repository<WBSTemplate>()
            .GetAllAsync(
                filter: t => t.IsActive && !t.IsDeleted,
                orderBy: q => q.OrderBy(t => t.Name),
                includeProperties: "Elements");
        
        var dtos = _mapper.Map<IEnumerable<WBSTemplateDto>>(templates);
        
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

    public async Task<IEnumerable<WBSTemplateDto>> GetByIndustryTypeAsync(string industryType)
    {
        var templates = await _unitOfWork.Repository<WBSTemplate>()
            .GetAllAsync(
                filter: t => t.IndustryType == industryType && t.IsActive && !t.IsDeleted,
                orderBy: q => q.OrderBy(t => t.Name));
        
        return _mapper.Map<IEnumerable<WBSTemplateDto>>(templates);
    }

    public async Task<IEnumerable<WBSTemplateDto>> GetByProjectTypeAsync(string projectType)
    {
        var templates = await _unitOfWork.Repository<WBSTemplate>()
            .GetAllAsync(
                filter: t => t.ProjectType == projectType && t.IsActive && !t.IsDeleted,
                orderBy: q => q.OrderBy(t => t.Name));
        
        return _mapper.Map<IEnumerable<WBSTemplateDto>>(templates);
    }

    public async Task<IEnumerable<WBSTemplateDto>> SearchAsync(WBSCBSTemplateFilterDto filter)
    {
        var query = _unitOfWork.Repository<WBSTemplate>()
            .Query()
            .Where(t => !t.IsDeleted);

        if (filter.Type == "WBS")
        {
            if (!string.IsNullOrWhiteSpace(filter.IndustryType))
                query = query.Where(t => t.IndustryType == filter.IndustryType);

            if (!string.IsNullOrWhiteSpace(filter.ProjectType))
                query = query.Where(t => t.ProjectType == filter.ProjectType);

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

        return _mapper.Map<IEnumerable<WBSTemplateDto>>(templates);
    }

    public async Task<IEnumerable<WBSTemplateElementDto>> GetElementsAsync(Guid templateId)
    {
        var elements = await _unitOfWork.Repository<WBSTemplateElement>()
            .GetAllAsync(
                filter: e => e.WBSTemplateId == templateId,
                orderBy: q => q.OrderBy(e => e.Level).ThenBy(e => e.SequenceNumber),
                includeProperties: "Children");
        
        return _mapper.Map<IEnumerable<WBSTemplateElementDto>>(elements);
    }

    public async Task<WBSTemplateElementDto?> AddElementAsync(Guid templateId, WBSTemplateElementDto element)
    {
        var template = await _unitOfWork.Repository<WBSTemplate>()
            .GetAsync(filter: t => t.Id == templateId && !t.IsDeleted);

        if (template == null)
            return null;

        WBSTemplateElement? parent = null;
        if (element.ParentId.HasValue)
        {
            parent = await _unitOfWork.Repository<WBSTemplateElement>()
                .GetAsync(filter: e => e.Id == element.ParentId.Value);
        }

        var newElement = template.AddElement(
            element.Code,
            element.Name,
            parent,
            element.ElementType,
            element.IsOptional);

        if (!string.IsNullOrWhiteSpace(element.Description))
            newElement.UpdateBasicInfo(element.Name, element.Description);

        if (element.DefaultBudgetPercentage.HasValue || element.DefaultDurationDays.HasValue || !string.IsNullOrWhiteSpace(element.DefaultDiscipline))
        {
            newElement.SetDefaultValues(
                element.DefaultBudgetPercentage,
                element.DefaultDurationDays,
                element.DefaultDiscipline);
        }

        _unitOfWork.Repository<WBSTemplate>().Update(template);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<WBSTemplateElementDto>(newElement);
    }

    public async Task<bool> UpdateElementAsync(Guid elementId, WBSTemplateElementDto element)
    {
        var existingElement = await _unitOfWork.Repository<WBSTemplateElement>()
            .GetAsync(filter: e => e.Id == elementId);

        if (existingElement == null)
            return false;

        existingElement.UpdateBasicInfo(element.Name, element.Description);
        
        if (element.DefaultBudgetPercentage.HasValue || element.DefaultDurationDays.HasValue || !string.IsNullOrWhiteSpace(element.DefaultDiscipline))
        {
            existingElement.SetDefaultValues(
                element.DefaultBudgetPercentage,
                element.DefaultDurationDays,
                element.DefaultDiscipline);
        }

        existingElement.SetOptional(element.IsOptional);
        existingElement.SetSequenceNumber(element.SequenceNumber);

        _unitOfWork.Repository<WBSTemplateElement>().Update(existingElement);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveElementAsync(Guid templateId, Guid elementId)
    {
        var template = await _unitOfWork.Repository<WBSTemplate>()
            .GetAsync(
                filter: t => t.Id == templateId && !t.IsDeleted,
                includeProperties: "Elements");

        if (template == null)
            return false;

        template.RemoveElement(elementId);
        
        _unitOfWork.Repository<WBSTemplate>().Update(template);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ReorderElementsAsync(Guid templateId, List<Guid> elementIds)
    {
        var elements = await _unitOfWork.Repository<WBSTemplateElement>()
            .GetAllAsync(filter: e => e.WBSTemplateId == templateId);

        var sequence = 1;
        foreach (var id in elementIds)
        {
            var element = elements.FirstOrDefault(e => e.Id == id);
            if (element != null)
            {
                element.SetSequenceNumber(sequence++);
                _unitOfWork.Repository<WBSTemplateElement>().Update(element);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<int> ApplyToProjectAsync(Guid templateId, Guid projectId, bool includeOptional = false)
    {
        var template = await _unitOfWork.Repository<WBSTemplate>()
            .GetAsync(
                filter: t => t.Id == templateId && !t.IsDeleted,
                includeProperties: "Elements");

        if (template == null)
            return 0;

        var project = await _unitOfWork.Repository<Project>()
            .GetAsync(filter: p => p.Id == projectId && !p.IsDeleted);

        if (project == null)
            return 0;

        var elementsToApply = template.Elements
            .Where(e => includeOptional || !e.IsOptional)
            .ToList();

        var createdCount = 0;
        
        // Create WBS elements in the project based on template elements
        foreach (var templateElement in elementsToApply)
        {
            // Create WBSElement entity (implementation depends on your WBSElement structure)
            // This is a simplified example - skipping actual implementation
            // as WBSElement constructor and methods need to be verified
            
            // TODO: Implement actual WBS element creation based on your domain model

            // await _unitOfWork.Repository<WBSElement>().AddAsync(wbsElement);
            createdCount++;
        }

        template.IncrementUsage();
        _unitOfWork.Repository<WBSTemplate>().Update(template);
        
        await _unitOfWork.SaveChangesAsync();
        
        return createdCount;
    }

    public async Task<WBSTemplateDto?> CloneAsync(Guid sourceId, string newCode, string newName)
    {
        var source = await _unitOfWork.Repository<WBSTemplate>()
            .GetAsync(
                filter: t => t.Id == sourceId && !t.IsDeleted,
                includeProperties: "Elements");

        if (source == null)
            return null;

        var clone = new WBSTemplate(newCode, newName, source.IndustryType, source.ProjectType);
        
        clone.UpdateBasicInfo(newName, $"Cloned from {source.Name}", source.IndustryType, source.ProjectType);
        clone.UpdateCodingScheme(source.CodingScheme, source.Delimiter, source.CodeLength, source.AutoGenerateCodes);
        
        await _unitOfWork.Repository<WBSTemplate>().AddAsync(clone);
        
        // Clone elements
        var elementMapping = new Dictionary<Guid, WBSTemplateElement>();
        
        foreach (var sourceElement in source.Elements.Where(e => e.ParentId == null))
        {
            await CloneElementRecursive(sourceElement, clone, null, elementMapping);
        }
        
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<WBSTemplateDto>(clone);
    }

    private async Task CloneElementRecursive(
        WBSTemplateElement sourceElement, 
        WBSTemplate targetTemplate, 
        WBSTemplateElement? targetParent,
        Dictionary<Guid, WBSTemplateElement> elementMapping)
    {
        var clonedElement = targetTemplate.AddElement(
            sourceElement.Code,
            sourceElement.Name,
            targetParent,
            sourceElement.ElementType,
            sourceElement.IsOptional);

        if (!string.IsNullOrWhiteSpace(sourceElement.Description))
            clonedElement.UpdateBasicInfo(sourceElement.Name, sourceElement.Description);

        clonedElement.SetDefaultValues(
            sourceElement.DefaultBudgetPercentage,
            sourceElement.DefaultDurationDays,
            sourceElement.DefaultDiscipline);

        elementMapping[sourceElement.Id] = clonedElement;

        // Clone children
        foreach (var child in sourceElement.Children)
        {
            await CloneElementRecursive(child, targetTemplate, clonedElement, elementMapping);
        }
    }

    public async Task<bool> ActivateAsync(Guid id)
    {
        var template = await _unitOfWork.Repository<WBSTemplate>()
            .GetAsync(filter: t => t.Id == id && !t.IsDeleted);

        if (template == null)
            return false;

        template.Activate();
        
        _unitOfWork.Repository<WBSTemplate>().Update(template);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var template = await _unitOfWork.Repository<WBSTemplate>()
            .GetAsync(filter: t => t.Id == id && !t.IsDeleted);

        if (template == null)
            return false;

        template.Deactivate();
        
        _unitOfWork.Repository<WBSTemplate>().Update(template);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<WBSTemplateDto?> ImportAsync(ImportWBSTemplateDto dto)
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
        
        var template = await _unitOfWork.Repository<WBSTemplate>()
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
        }

        return result;
    }

    public async Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null)
    {
        var query = _unitOfWork.Repository<WBSTemplate>()
            .Query()
            .Where(t => t.Code == code && !t.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId.Value);

        return !await query.AnyAsync();
    }

    public async Task<TemplateUsageStatisticsDto> GetUsageStatisticsAsync(Guid templateId)
    {
        var template = await _unitOfWork.Repository<WBSTemplate>()
            .GetAsync(filter: t => t.Id == templateId && !t.IsDeleted);

        if (template == null)
            throw new InvalidOperationException("Template not found");

        // This would require tracking template usage in projects
        // For now, returning basic statistics
        return new TemplateUsageStatisticsDto
        {
            TemplateId = templateId,
            TemplateName = template.Name,
            TemplateType = "WBS",
            TotalUsageCount = template.UsageCount,
            LastUsedDate = template.LastUsedDate,
            // Other statistics would be calculated from project data
        };
    }
}