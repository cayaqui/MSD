using Application.Interfaces.Cost;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.WorkPackages;
using Core.Enums.Projects;
using Core.Enums.Progress;
using Domain.Entities.WBS;
using Domain.Entities.Progress;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Cost;

/// <summary>
/// Service implementation for Work Package management
/// </summary>
public class WorkPackageService : IWorkPackageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<WorkPackageService> _logger;

    public WorkPackageService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<WorkPackageService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // Query Operations
    public async Task<PagedResult<WorkPackageDto>> GetWorkPackagesAsync(
        Guid projectId,
        QueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting work packages for project {ProjectId}", projectId);

        var query = _unitOfWork.Repository<WBSElement>()
            .Query()
            .Where(w => w.ProjectId == projectId && w.ElementType == WBSElementType.WorkPackage)
            .Include(w => w.WorkPackageDetails)
                .ThenInclude(wp => wp!.ResponsibleUser)
            .Include(w => w.ControlAccount)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            query = query.Where(w => 
                w.Code.Contains(parameters.SearchTerm) ||
                w.Name.Contains(parameters.SearchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = parameters.SortBy?.ToLower() switch
        {
            "code" => !parameters.IsAscending 
                ? query.OrderByDescending(w => w.Code) 
                : query.OrderBy(w => w.Code),
            "name" => !parameters.IsAscending 
                ? query.OrderByDescending(w => w.Name) 
                : query.OrderBy(w => w.Name),
            "progress" => !parameters.IsAscending 
                ? query.OrderByDescending(w => w.WorkPackageDetails!.ProgressPercentage) 
                : query.OrderBy(w => w.WorkPackageDetails!.ProgressPercentage),
            _ => query.OrderBy(w => w.Code)
        };

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var mappedItems = items.Select(wbs => new WorkPackageDto
        {
            Id = wbs.Id,
            Code = wbs.Code,
            Name = wbs.Name,
            Description = wbs.Description,
            ControlAccountId = wbs.ControlAccountId ?? Guid.Empty,
            ControlAccountCode = wbs.ControlAccount?.Code ?? string.Empty,
            ControlAccountName = wbs.ControlAccount?.Name ?? string.Empty,
            ProjectId = wbs.ProjectId,
            ProjectName = wbs.Project?.Name ?? string.Empty,
            ResponsibleUserId = wbs.WorkPackageDetails?.ResponsibleUserId?.ToString(),
            ResponsibleUserName = wbs.WorkPackageDetails?.ResponsibleUser?.Name,
            Budget = wbs.WorkPackageDetails?.Budget ?? 0,
            ActualCost = wbs.WorkPackageDetails?.ActualCost ?? 0,
            CommittedCost = wbs.WorkPackageDetails?.CommittedCost ?? 0,
            ForecastCost = wbs.WorkPackageDetails?.ForecastCost ?? 0,
            ProgressPercentage = wbs.WorkPackageDetails?.ProgressPercentage ?? 0,
            Status = wbs.WorkPackageDetails?.Status ?? WorkPackageStatus.NotStarted,
            PlannedStartDate = wbs.WorkPackageDetails?.PlannedStartDate ?? DateTime.Now,
            PlannedEndDate = wbs.WorkPackageDetails?.PlannedEndDate ?? DateTime.Now.AddDays(30),
            ActualStartDate = wbs.WorkPackageDetails?.ActualStartDate,
            ActualEndDate = wbs.WorkPackageDetails?.ActualEndDate,
            IsBaselined = wbs.WorkPackageDetails?.IsBaselined ?? false,
            IsActive = wbs.IsActive
        }).ToList();

        return new PagedResult<WorkPackageDto>
        {
            Items = mappedItems,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize)
        };
    }

    public async Task<WorkPackageDetailDto?> GetWorkPackageByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting work package {Id}", id);

        var wbs = await _unitOfWork.Repository<WBSElement>()
            .Query()
            .Where(w => w.Id == id && w.ElementType == WBSElementType.WorkPackage)
            .Include(w => w.WorkPackageDetails)
                .ThenInclude(wp => wp!.ResponsibleUser)
            .Include(w => w.WorkPackageDetails)
                .ThenInclude(wp => wp!.PrimaryDiscipline)
            .Include(w => w.ControlAccount)
            .Include(w => w.Project)
            .FirstOrDefaultAsync(cancellationToken);

        if (wbs == null) return null;

        // Get activities
        var activities = await _unitOfWork.Repository<Activity>()
            .Query()
            .Where(a => a.WBSElementId == id)
            .ToListAsync(cancellationToken);

        // Get latest progress
        var latestProgress = await _unitOfWork.Repository<WorkPackageProgress>()
            .Query()
            .Where(p => p.WorkPackageId == id)
            .OrderByDescending(p => p.ProgressDate)
            .FirstOrDefaultAsync(cancellationToken);

        // Map to detail DTO
        var dto = new WorkPackageDetailDto
        {
            Id = wbs.Id,
            Code = wbs.Code,
            Name = wbs.Name,
            Description = wbs.Description,
            ControlAccountId = wbs.ControlAccountId ?? Guid.Empty,
            ControlAccountCode = wbs.ControlAccount?.Code ?? string.Empty,
            ControlAccountName = wbs.ControlAccount?.Name ?? string.Empty,
            ProjectId = wbs.ProjectId,
            ProjectName = wbs.Project?.Name ?? string.Empty,
            ResponsibleUserId = wbs.WorkPackageDetails?.ResponsibleUserId?.ToString(),
            ResponsibleUserName = wbs.WorkPackageDetails?.ResponsibleUser?.Name,
            Budget = wbs.WorkPackageDetails?.Budget ?? 0,
            ActualCost = wbs.WorkPackageDetails?.ActualCost ?? 0,
            CommittedCost = wbs.WorkPackageDetails?.CommittedCost ?? 0,
            ForecastCost = wbs.WorkPackageDetails?.ForecastCost ?? 0,
            RemainingCost = (wbs.WorkPackageDetails?.Budget ?? 0) - (wbs.WorkPackageDetails?.ActualCost ?? 0),
            ProgressPercentage = wbs.WorkPackageDetails?.ProgressPercentage ?? 0,
            Status = wbs.WorkPackageDetails?.Status ?? WorkPackageStatus.NotStarted,
            PlannedStartDate = wbs.WorkPackageDetails?.PlannedStartDate ?? DateTime.Now,
            PlannedEndDate = wbs.WorkPackageDetails?.PlannedEndDate ?? DateTime.Now.AddDays(30),
            ActualStartDate = wbs.WorkPackageDetails?.ActualStartDate,
            ActualEndDate = wbs.WorkPackageDetails?.ActualEndDate,
            IsBaselined = wbs.WorkPackageDetails?.IsBaselined ?? false,
            IsActive = wbs.IsActive,
            WBSLevel = wbs.Level,
            DisciplineId = wbs.WorkPackageDetails?.PrimaryDisciplineId,
            DisciplineName = wbs.WorkPackageDetails?.PrimaryDiscipline?.Name,
            PlannedDuration = wbs.WorkPackageDetails?.PlannedDuration ?? 0,
            ActualDuration = wbs.WorkPackageDetails?.ActualDuration,
            ProgressMethod = wbs.WorkPackageDetails?.ProgressMethod ?? ProgressMethod.Manual,
            WeightFactor = wbs.WorkPackageDetails?.WeightFactor,
            BaselineDate = wbs.WorkPackageDetails?.BaselineDate,
            CostVariance = (wbs.WorkPackageDetails?.Budget ?? 0) - (wbs.WorkPackageDetails?.ActualCost ?? 0),
            ScheduleVariance = 0, // Calcular basado en valor ganado
            Activities = _mapper.Map<List<ActivityDto>>(activities),
            LatestProgress = latestProgress != null ? _mapper.Map<WorkPackageProgressDto>(latestProgress) : null,
            CreatedAt = wbs.CreatedAt,
            UpdatedAt = wbs.UpdatedAt
        };

        return dto;
    }

    public async Task<List<WorkPackageDto>> GetWorkPackagesByControlAccountAsync(
        Guid controlAccountId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting work packages for control account {ControlAccountId}", controlAccountId);

        var workPackages = await _unitOfWork.Repository<WBSElement>()
            .Query()
            .Where(w => w.ControlAccountId == controlAccountId && 
                       w.ElementType == WBSElementType.WorkPackage)
            .Include(w => w.WorkPackageDetails)
                .ThenInclude(wp => wp!.ResponsibleUser)
            .Include(w => w.ControlAccount)
            .OrderBy(w => w.Code)
            .ToListAsync(cancellationToken);

        return workPackages.Select(wbs => new WorkPackageDto
        {
            Id = wbs.Id,
            Code = wbs.Code,
            Name = wbs.Name,
            Description = wbs.Description,
            ControlAccountId = wbs.ControlAccountId ?? Guid.Empty,
            ControlAccountCode = wbs.ControlAccount?.Code ?? string.Empty,
            ControlAccountName = wbs.ControlAccount?.Name ?? string.Empty,
            ProjectId = wbs.ProjectId,
            ResponsibleUserId = wbs.WorkPackageDetails?.ResponsibleUserId?.ToString(),
            ResponsibleUserName = wbs.WorkPackageDetails?.ResponsibleUser?.Name,
            Budget = wbs.WorkPackageDetails?.Budget ?? 0,
            ActualCost = wbs.WorkPackageDetails?.ActualCost ?? 0,
            CommittedCost = wbs.WorkPackageDetails?.CommittedCost ?? 0,
            ForecastCost = wbs.WorkPackageDetails?.ForecastCost ?? 0,
            ProgressPercentage = wbs.WorkPackageDetails?.ProgressPercentage ?? 0,
            Status = wbs.WorkPackageDetails?.Status ?? WorkPackageStatus.NotStarted,
            PlannedStartDate = wbs.WorkPackageDetails?.PlannedStartDate ?? DateTime.Now,
            PlannedEndDate = wbs.WorkPackageDetails?.PlannedEndDate ?? DateTime.Now.AddDays(30),
            ActualStartDate = wbs.WorkPackageDetails?.ActualStartDate,
            ActualEndDate = wbs.WorkPackageDetails?.ActualEndDate,
            IsBaselined = wbs.WorkPackageDetails?.IsBaselined ?? false,
            IsActive = wbs.IsActive
        }).ToList();
    }

    public async Task<List<WorkPackageProgressDto>> GetWorkPackageProgressHistoryAsync(
        Guid workPackageId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting progress history for work package {WorkPackageId}", workPackageId);

        var progressHistory = await _unitOfWork.Repository<WorkPackageProgress>()
            .Query()
            .Where(p => p.WorkPackageId == workPackageId)
            .OrderByDescending(p => p.ProgressDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<WorkPackageProgressDto>>(progressHistory);
    }

    // Command Operations
    public async Task<Result<Guid>> CreateWorkPackageAsync(
        CreateWorkPackageDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating work package for project {ProjectId}", dto.ProjectId);

            // Validate code uniqueness
            var codeExists = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .AnyAsync(w => w.ProjectId == dto.ProjectId && w.Code == dto.Code, cancellationToken);

            if (codeExists)
            {
                return Result<Guid>.Failure($"El código '{dto.Code}' ya existe en el proyecto");
            }

            // Create WBS element as work package directly
            var wbsElement = new WBSElement(
                dto.ProjectId,
                dto.Code,
                dto.Name,
                WBSElementType.WorkPackage,
                1, // Default level
                1, // Default sequence
                null); // No parent for direct creation

            if (!string.IsNullOrEmpty(dto.Description))
            {
                wbsElement.UpdateBasicInfo(dto.Name, dto.Description);
            }

            // Convert to work package with details
            wbsElement.ConvertToWorkPackage(dto.ControlAccountId, dto.ProgressMethod);
            
            // Set work package details
            if (wbsElement.WorkPackageDetails != null)
            {
                wbsElement.WorkPackageDetails.UpdateSchedule(
                    dto.PlannedStartDate, 
                    dto.PlannedEndDate, 
                    userId);
                
                // Update budget using UpdateCosts method
                wbsElement.WorkPackageDetails.UpdateCosts(0, dto.Budget, userId);
                
                if (!string.IsNullOrEmpty(dto.ResponsibleUserId))
                {
                    if (Guid.TryParse(dto.ResponsibleUserId, out var responsibleGuid))
                    {
                        wbsElement.WorkPackageDetails.AssignResponsibility(
                            responsibleGuid, 
                            dto.DisciplineId, 
                            userId);
                    }
                }
                
                // WeightFactor is a read-only property in WorkPackageDetails
                // We would need to modify the domain model to allow setting it
            }

            wbsElement.CreatedBy = userId;
            wbsElement.UpdatedBy = userId;

            await _unitOfWork.Repository<WBSElement>().AddAsync(wbsElement, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created work package {Id} with code {Code}", wbsElement.Id, wbsElement.Code);
            return Result<Guid>.Success(wbsElement.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating work package");
            return Result<Guid>.Failure($"Error al crear el paquete de trabajo: {ex.Message}");
        }
    }

    public async Task<Result> UpdateWorkPackageAsync(
        Guid id,
        UpdateWorkPackageDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating work package {Id}", id);

            var wbsElement = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.WorkPackageDetails)
                .FirstOrDefaultAsync(w => w.Id == id && w.ElementType == WBSElementType.WorkPackage, cancellationToken);

            if (wbsElement == null)
            {
                return Result.Failure($"Paquete de trabajo {id} no encontrado");
            }

            // Update basic info
            if (!string.IsNullOrEmpty(dto.Name))
            {
                wbsElement.UpdateBasicInfo(dto.Name, dto.Description);
            }

            // Update work package details
            if (wbsElement.WorkPackageDetails != null)
            {
                if (!string.IsNullOrEmpty(dto.ResponsibleUserId))
                {
                    if (Guid.TryParse(dto.ResponsibleUserId, out var responsibleGuid))
                    {
                        wbsElement.WorkPackageDetails.AssignResponsibility(
                            responsibleGuid, 
                            dto.DisciplineId, 
                            userId);
                    }
                }

                if (dto.Budget.HasValue)
                {
                    wbsElement.WorkPackageDetails.UpdateCosts(
                        wbsElement.WorkPackageDetails.CommittedCost, 
                        dto.Budget.Value, 
                        userId);
                }

                if (dto.PlannedStartDate.HasValue && dto.PlannedEndDate.HasValue)
                {
                    wbsElement.WorkPackageDetails.UpdateSchedule(
                        dto.PlannedStartDate.Value, 
                        dto.PlannedEndDate.Value, 
                        userId);
                }

                // WeightFactor cannot be updated directly
            }

            wbsElement.UpdatedBy = userId;
            _unitOfWork.Repository<WBSElement>().Update(wbsElement);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated work package {Id}", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating work package {Id}", id);
            return Result.Failure($"Error al actualizar el paquete de trabajo: {ex.Message}");
        }
    }

    public async Task<Result> UpdateWorkPackageProgressAsync(
        Guid id,
        UpdateWorkPackageProgressDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating progress for work package {Id}", id);

            var wbsElement = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.WorkPackageDetails)
                .FirstOrDefaultAsync(w => w.Id == id && w.ElementType == WBSElementType.WorkPackage, cancellationToken);

            if (wbsElement?.WorkPackageDetails == null)
            {
                return Result.Failure($"Paquete de trabajo {id} no encontrado");
            }

            // Get previous progress
            var previousProgress = wbsElement.WorkPackageDetails.ProgressPercentage;
            var previousActualCost = wbsElement.WorkPackageDetails.ActualCost;

            // Update work package progress
            wbsElement.WorkPackageDetails.UpdateProgress(
                dto.ProgressPercentage,
                dto.ProgressPercentage, // Physical progress same as regular progress
                dto.ActualCost,
                userId);
            
            // Update committed cost
            wbsElement.WorkPackageDetails.UpdateCosts(dto.CommittedCost, wbsElement.WorkPackageDetails.ForecastCost, userId);

            // Create progress record
            var progressRecord = new WorkPackageProgress(
                id,
                dto.ProgressDate,
                previousProgress,
                dto.ProgressPercentage,
                previousActualCost,
                dto.ActualCost,
                dto.CommittedCost,
                wbsElement.WorkPackageDetails.ForecastCost,
                wbsElement.WorkPackageDetails.ProgressMethod,
                dto.Comments);

            progressRecord.CreatedBy = userId;

            await _unitOfWork.Repository<WorkPackageProgress>().AddAsync(progressRecord, cancellationToken);
            _unitOfWork.Repository<WBSElement>().Update(wbsElement);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated progress for work package {Id} to {Progress}%", id, dto.ProgressPercentage);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating work package progress {Id}", id);
            return Result.Failure($"Error al actualizar el progreso: {ex.Message}");
        }
    }

    public async Task<Result> StartWorkPackageAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting work package {Id}", id);

            var wbsElement = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.WorkPackageDetails)
                .FirstOrDefaultAsync(w => w.Id == id && w.ElementType == WBSElementType.WorkPackage, cancellationToken);

            if (wbsElement?.WorkPackageDetails == null)
            {
                return Result.Failure($"Paquete de trabajo {id} no encontrado");
            }

            wbsElement.WorkPackageDetails.Start(userId);
            wbsElement.UpdatedBy = userId;

            _unitOfWork.Repository<WBSElement>().Update(wbsElement);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Started work package {Id}", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting work package {Id}", id);
            return Result.Failure($"Error al iniciar el paquete de trabajo: {ex.Message}");
        }
    }

    public async Task<Result> CompleteWorkPackageAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Completing work package {Id}", id);

            var wbsElement = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.WorkPackageDetails)
                .FirstOrDefaultAsync(w => w.Id == id && w.ElementType == WBSElementType.WorkPackage, cancellationToken);

            if (wbsElement?.WorkPackageDetails == null)
            {
                return Result.Failure($"Paquete de trabajo {id} no encontrado");
            }

            wbsElement.WorkPackageDetails.Complete(userId);
            wbsElement.UpdatedBy = userId;

            _unitOfWork.Repository<WBSElement>().Update(wbsElement);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Completed work package {Id}", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing work package {Id}", id);
            return Result.Failure($"Error al completar el paquete de trabajo: {ex.Message}");
        }
    }

    public async Task<Result> BaselineWorkPackageAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Baselining work package {Id}", id);

            var wbsElement = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.WorkPackageDetails)
                .FirstOrDefaultAsync(w => w.Id == id && w.ElementType == WBSElementType.WorkPackage, cancellationToken);

            if (wbsElement?.WorkPackageDetails == null)
            {
                return Result.Failure($"Paquete de trabajo {id} no encontrado");
            }

            wbsElement.WorkPackageDetails.Baseline(userId);
            wbsElement.UpdatedBy = userId;

            _unitOfWork.Repository<WBSElement>().Update(wbsElement);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Baselined work package {Id}", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error baselining work package {Id}", id);
            return Result.Failure($"Error al establecer línea base: {ex.Message}");
        }
    }

    public async Task<Result> DeleteWorkPackageAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting work package {Id}", id);

            var wbsElement = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.WorkPackageDetails)
                .FirstOrDefaultAsync(w => w.Id == id && w.ElementType == WBSElementType.WorkPackage, cancellationToken);

            if (wbsElement == null)
            {
                return Result.Failure($"Paquete de trabajo {id} no encontrado");
            }

            // Check if can delete
            if (wbsElement.WorkPackageDetails?.ActualCost > 0)
            {
                return Result.Failure("No se puede eliminar un paquete de trabajo con costos registrados");
            }

            if (wbsElement.WorkPackageDetails?.ProgressPercentage > 0)
            {
                return Result.Failure("No se puede eliminar un paquete de trabajo con progreso registrado");
            }

            _unitOfWork.Repository<WBSElement>().Remove(wbsElement);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted work package {Id}", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting work package {Id}", id);
            return Result.Failure($"Error al eliminar el paquete de trabajo: {ex.Message}");
        }
    }

    // Activity Operations
    public async Task<Result<Guid>> AddActivityToWorkPackageAsync(
        Guid workPackageId,
        CreateActivityDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding activity to work package {WorkPackageId}", workPackageId);

            var wbsElement = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .FirstOrDefaultAsync(w => w.Id == workPackageId && w.ElementType == WBSElementType.WorkPackage, cancellationToken);

            if (wbsElement == null)
            {
                return Result<Guid>.Failure($"Paquete de trabajo {workPackageId} no encontrado");
            }

            // Create activity
            var activity = new Activity(
                workPackageId,
                dto.ActivityCode,
                dto.Name,
                dto.PlannedStartDate,
                dto.PlannedEndDate,
                dto.PlannedHours);

            // Activity.Description is read-only, would need to modify domain model

            activity.CreatedBy = userId;

            await _unitOfWork.Repository<Activity>().AddAsync(activity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Added activity {ActivityId} to work package {WorkPackageId}", activity.Id, workPackageId);
            return Result<Guid>.Success(activity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding activity to work package {WorkPackageId}", workPackageId);
            return Result<Guid>.Failure($"Error al agregar actividad: {ex.Message}");
        }
    }

    public async Task<Result> UpdateActivityProgressAsync(
        Guid activityId,
        decimal percentComplete,
        decimal actualHours,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating progress for activity {ActivityId}", activityId);

            var activity = await _unitOfWork.Repository<Activity>()
                .GetByIdAsync(activityId, cancellationToken);

            if (activity == null)
            {
                return Result.Failure($"Actividad {activityId} no encontrada");
            }

            activity.UpdateProgress(percentComplete, actualHours);
            activity.UpdatedBy = userId;

            _unitOfWork.Repository<Activity>().Update(activity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated progress for activity {ActivityId} to {Progress}%", activityId, percentComplete);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating activity progress {ActivityId}", activityId);
            return Result.Failure($"Error al actualizar progreso de actividad: {ex.Message}");
        }
    }
}