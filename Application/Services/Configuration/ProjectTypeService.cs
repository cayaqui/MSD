using Application.Interfaces.Common;
using Application.Interfaces.Configuration;
using Application.Services.Base;
using AutoMapper;
using Core.DTOs.Configuration.ProjectTypes;
using Domain.Entities.Configuration.Core;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Configuration;

/// <summary>
/// Service for managing project types
/// </summary>
public class ProjectTypeService : BaseService<ProjectType, ProjectTypeDto, CreateProjectTypeDto, UpdateProjectTypeDto>, IProjectTypeService
{
    public ProjectTypeService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ProjectTypeService> logger) 
        : base(unitOfWork, mapper, logger)
    {
    }

    public async Task<ProjectTypeDto?> GetByCodeAsync(string code)
    {
        var projectType = await _unitOfWork.Repository<ProjectType>().Query()
            .FirstOrDefaultAsync(pt => pt.Code == code);
            
        return projectType != null ? _mapper.Map<ProjectTypeDto>(projectType) : null;
    }

    public async Task<IEnumerable<ProjectTypeDto>> GetActiveAsync()
    {
        var projectTypes = await _unitOfWork.Repository<ProjectType>().Query()
            .Where(pt => pt.IsActive)
            .OrderBy(pt => pt.Name)
            .ToListAsync();
            
        return _mapper.Map<IEnumerable<ProjectTypeDto>>(projectTypes);
    }

    public async Task<IEnumerable<ProjectTypeDto>> SearchAsync(ProjectTypeFilterDto filter)
    {
        var query = _unitOfWork.Repository<ProjectType>().Query();

        if (filter.IsActive.HasValue)
            query = query.Where(pt => pt.IsActive == filter.IsActive.Value);

        if (filter.RequiresSchedule.HasValue)
            query = query.Where(pt => pt.RequiresSchedule == filter.RequiresSchedule.Value);

        if (filter.RequiresBudget.HasValue)
            query = query.Where(pt => pt.RequiresBudget == filter.RequiresBudget.Value);

        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            query = query.Where(pt => 
                pt.Name.Contains(filter.SearchTerm) || 
                pt.Code.Contains(filter.SearchTerm) ||
                (pt.Description != null && pt.Description.Contains(filter.SearchTerm)));
        }

        var totalCount = await query.CountAsync();
        
        var projectTypes = await query
            .OrderBy(pt => pt.Name)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProjectTypeDto>>(projectTypes);
    }

    public async Task<bool> ActivateAsync(Guid id)
    {
        var projectType = await _unitOfWork.Repository<ProjectType>().GetByIdAsync(id);
        if (projectType == null) return false;

        projectType.Activate();
        _unitOfWork.Repository<ProjectType>().Update(projectType);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Project type {Id} activated", id);
        return true;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var projectType = await _unitOfWork.Repository<ProjectType>().GetByIdAsync(id);
        if (projectType == null) return false;

        projectType.Deactivate();
        _unitOfWork.Repository<ProjectType>().Update(projectType);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Project type {Id} deactivated", id);
        return true;
    }

    public async Task<bool> SetTemplatesAsync(Guid id, Guid? wbsTemplateId, Guid? cbsTemplateId, Guid? obsTemplateId, Guid? riskRegisterTemplateId)
    {
        var projectType = await _unitOfWork.Repository<ProjectType>().GetByIdAsync(id);
        if (projectType == null) return false;

        projectType.SetTemplates(wbsTemplateId, cbsTemplateId, obsTemplateId, riskRegisterTemplateId);
        _unitOfWork.Repository<ProjectType>().Update(projectType);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Templates set for project type {Id}", id);
        return true;
    }

    public async Task<bool> SetWorkflowStagesAsync(Guid id, List<WorkflowStageDto> stages)
    {
        var projectType = await _unitOfWork.Repository<ProjectType>().GetByIdAsync(id);
        if (projectType == null) return false;

        // Set workflow stages as JSON
        projectType.SetWorkflowStages(stages);
        _unitOfWork.Repository<ProjectType>().Update(projectType);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Workflow stages set for project type {Id}", id);
        return true;
    }

    public async Task<bool> SetApprovalLevelsAsync(Guid id, List<string> approvalLevels)
    {
        var projectType = await _unitOfWork.Repository<ProjectType>().GetByIdAsync(id);
        if (projectType == null) return false;

        projectType.SetApprovalLevels(approvalLevels);
        _unitOfWork.Repository<ProjectType>().Update(projectType);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Approval levels set for project type {Id}", id);
        return true;
    }

    public async Task<ProjectTypeDto?> CloneAsync(Guid sourceId, string newCode, string newName)
    {
        var source = await _unitOfWork.Repository<ProjectType>().GetByIdAsync(sourceId);
        if (source == null) return null;

        var clone = new ProjectType(newCode, newName);
        
        // Update basic info
        clone.UpdateBasicInfo(newName, source.Description, source.Icon, source.Color);

        // Copy configuration settings
        clone.UpdateModuleRequirements(
            source.RequiresWBS,
            source.RequiresCBS,
            source.RequiresOBS,
            source.RequiresSchedule,
            source.RequiresBudget,
            source.RequiresRiskManagement,
            source.RequiresDocumentControl,
            source.RequiresChangeManagement,
            source.RequiresQualityControl,
            source.RequiresHSE
        );

        // Copy default settings
        clone.UpdateDefaultSettings(
            source.DefaultDurationUnit,
            source.DefaultCurrency,
            source.DefaultProgressMethod,
            source.DefaultContingencyPercentage,
            source.DefaultReportingPeriod
        );

        // Copy templates
        clone.SetTemplates(
            source.WBSTemplateId,
            source.CBSTemplateId,
            source.OBSTemplateId,
            source.RiskRegisterTemplateId
        );

        // Copy workflow stages
        if (!string.IsNullOrEmpty(source.WorkflowStagesJson))
        {
            var stages = System.Text.Json.JsonSerializer.Deserialize<List<WorkflowStageDto>>(source.WorkflowStagesJson);
            if (stages != null)
                clone.SetWorkflowStages(stages);
        }

        // Copy approval levels
        var approvalLevels = source.GetApprovalLevels();
        if (approvalLevels.Any())
        {
            clone.SetApprovalLevels(approvalLevels);
        }

        await _unitOfWork.Repository<ProjectType>().AddAsync(clone);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Project type {SourceId} cloned to {NewCode}", sourceId, newCode);
        return _mapper.Map<ProjectTypeDto>(clone);
    }

    public async Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null)
    {
        var query = _unitOfWork.Repository<ProjectType>().Query().Where(pt => pt.Code == code);
        
        if (excludeId.HasValue)
            query = query.Where(pt => pt.Id != excludeId.Value);

        return !await query.AnyAsync();
    }

    public override async Task<ProjectTypeDto> CreateAsync(
        CreateProjectTypeDto createDto,
        string? createdBy = null,
        CancellationToken cancellationToken = default)
    {
        // Check if code is unique
        if (!await IsCodeUniqueAsync(createDto.Code))
        {
            _logger.LogWarning("Project type code {Code} already exists", createDto.Code);
            return null;
        }

        var projectType = new ProjectType(
            createDto.Code,
            createDto.Name
        );

        // Update basic info with description and appearance
        projectType.UpdateBasicInfo(createDto.Name, createDto.Description, createDto.Icon, createDto.Color);

        // Set requirements
        projectType.UpdateModuleRequirements(
            createDto.RequiresWBS,
            createDto.RequiresCBS,
            createDto.RequiresOBS,
            createDto.RequiresSchedule,
            createDto.RequiresBudget,
            createDto.RequiresRiskManagement,
            createDto.RequiresDocumentControl,
            createDto.RequiresChangeManagement,
            createDto.RequiresQualityControl,
            createDto.RequiresHSE
        );

        // Set defaults
        projectType.UpdateDefaultSettings(
            createDto.DefaultDurationUnit,
            createDto.DefaultCurrency,
            createDto.DefaultProgressMethod,
            createDto.DefaultContingencyPercentage,
            createDto.DefaultReportingPeriod
        );

        await _unitOfWork.Repository<ProjectType>().AddAsync(projectType);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProjectTypeDto>(projectType);
    }
}