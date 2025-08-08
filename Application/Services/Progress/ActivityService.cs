using Application.Interfaces;
using Application.Interfaces.Progress;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.Progress.Activities;
using Core.Enums.Progress;
using Domain.Entities.Progress;
using Domain.Entities.WBS;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Result = Core.Results.Result;

namespace Application.Services.Progress;

public class ActivityService : IActivityService
{
    private readonly IRepository<Activity> _activityRepository;
    private readonly IRepository<WBSElement> _wbsRepository;
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ActivityService(
        IRepository<Activity> activityRepository,
        IRepository<WBSElement> wbsRepository,
        IRepository<Resource> resourceRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _activityRepository = activityRepository;
        _wbsRepository = wbsRepository;
        _resourceRepository = resourceRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<ActivityDto>> GetActivitiesAsync(ActivityFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = _activityRepository.GetAllQueryable()
            .Include(a => a.WBSElement)
            .Include(a => a.Resources)
            .AsQueryable();

        if (filter.ProjectId.HasValue)
            query = query.Where(a => a.WBSElement.ProjectId == filter.ProjectId.Value);

        if (filter.WBSElementId.HasValue)
            query = query.Where(a => a.WBSElementId == filter.WBSElementId.Value);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            query = query.Where(a => a.ActivityCode.Contains(filter.SearchTerm) || 
                                    a.Name.Contains(filter.SearchTerm) ||
                                    (a.Description != null && a.Description.Contains(filter.SearchTerm)));

        if (filter.Status.HasValue)
            query = query.Where(a => a.Status == filter.Status.Value);

        if (filter.StartDateFrom.HasValue)
            query = query.Where(a => a.PlannedStartDate >= filter.StartDateFrom.Value);

        if (filter.StartDateTo.HasValue)
            query = query.Where(a => a.PlannedStartDate <= filter.StartDateTo.Value);

        if (filter.MinProgress.HasValue)
            query = query.Where(a => a.ProgressPercentage >= filter.MinProgress.Value);

        if (filter.MaxProgress.HasValue)
            query = query.Where(a => a.ProgressPercentage <= filter.MaxProgress.Value);

        if (filter.IsCritical.HasValue && filter.IsCritical.Value)
            query = query.Where(a => a.DurationDays > 20); // Simplified critical path logic

        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = filter.SortBy?.ToLower() switch
        {
            "code" => filter.IsDescending ? query.OrderByDescending(a => a.ActivityCode) : query.OrderBy(a => a.ActivityCode),
            "name" => filter.IsDescending ? query.OrderByDescending(a => a.Name) : query.OrderBy(a => a.Name),
            "startdate" => filter.IsDescending ? query.OrderByDescending(a => a.PlannedStartDate) : query.OrderBy(a => a.PlannedStartDate),
            "progress" => filter.IsDescending ? query.OrderByDescending(a => a.ProgressPercentage) : query.OrderBy(a => a.ProgressPercentage),
            "status" => filter.IsDescending ? query.OrderByDescending(a => a.Status) : query.OrderBy(a => a.Status),
            _ => filter.IsDescending ? query.OrderByDescending(a => a.PlannedStartDate) : query.OrderBy(a => a.PlannedStartDate)
        };

        // Apply pagination
        var activities = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        var activityDtos = _mapper.Map<List<ActivityDto>>(activities);

        return new PagedResult<ActivityDto>(
            activityDtos,
            totalCount,
            filter.PageNumber,
            filter.PageSize);
    }

    public async Task<ActivityDto?> GetActivityByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var activity = await _activityRepository.GetAllQueryable()
            .Include(a => a.WBSElement)
            .Include(a => a.Resources)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return activity == null ? null : _mapper.Map<ActivityDto>(activity);
    }

    public async Task<List<ActivityDto>> GetActivitiesByWBSElementAsync(Guid wbsElementId, CancellationToken cancellationToken = default)
    {
        var activities = await _activityRepository.GetAllQueryable()
            .Include(a => a.WBSElement)
            .Include(a => a.Resources)
            .Where(a => a.WBSElementId == wbsElementId)
            .OrderBy(a => a.PlannedStartDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<ActivityDto>>(activities);
    }

    public async Task<List<ActivityDto>> GetCriticalPathActivitiesAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        // Simplified critical path logic - in reality this would be much more complex
        var activities = await _activityRepository.GetAllQueryable()
            .Include(a => a.WBSElement)
            .Include(a => a.Resources)
            .Where(a => a.WBSElement.ProjectId == projectId && a.DurationDays > 20)
            .OrderBy(a => a.PlannedStartDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<ActivityDto>>(activities);
    }

    public async Task<Core.Results.Result<Guid>> CreateActivityAsync(CreateActivityDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate WBS element exists
            var wbsElement = await _wbsRepository.GetByIdAsync(dto.WBSElementId, cancellationToken);
            if (wbsElement == null)
                return Result<Guid>.Failure("WBS element not found");

            // Validate activity code is unique within project
            var codeExists = await _activityRepository.GetAllQueryable()
                .Include(a => a.WBSElement)
                .AnyAsync(a => a.WBSElement.ProjectId == wbsElement.ProjectId && a.ActivityCode == dto.ActivityCode, cancellationToken);

            if (codeExists)
                return Result<Guid>.Failure("Activity code already exists in this project");

            var activity = new Activity(
                dto.WBSElementId,
                dto.ActivityCode,
                dto.Name,
                dto.PlannedStartDate,
                dto.PlannedEndDate,
                dto.PlannedHours);

            if (!string.IsNullOrWhiteSpace(dto.Description))
            {
                var descProp = typeof(Activity).GetProperty("Description");
                descProp?.SetValue(activity, dto.Description);
            }

            if (dto.ResourceRate.HasValue)
            {
                activity.SetResourceRate(dto.ResourceRate.Value);
            }

            if (dto.Predecessors?.Any() == true || dto.Successors?.Any() == true)
            {
                activity.SetDependencies(dto.Predecessors, dto.Successors);
            }

            await _activityRepository.AddAsync(activity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(activity.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to create activity: {ex.Message}");
        }
    }

    public async Task<Result> UpdateActivityAsync(Guid id, UpdateActivityDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var activity = await _activityRepository.GetByIdAsync(id, cancellationToken);
            if (activity == null)
                return Result.Failure("Activity not found");

            // Update basic properties using reflection for private setters
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                var nameProp = typeof(Activity).GetProperty("Name");
                nameProp?.SetValue(activity, dto.Name);
            }

            if (!string.IsNullOrWhiteSpace(dto.Description))
            {
                var descProp = typeof(Activity).GetProperty("Description");
                descProp?.SetValue(activity, dto.Description);
            }

            if (dto.PlannedStartDate.HasValue && dto.PlannedEndDate.HasValue)
            {
                activity.UpdateSchedule(dto.PlannedStartDate.Value, dto.PlannedEndDate.Value);
            }

            if (dto.PlannedHours.HasValue)
            {
                var hoursProp = typeof(Activity).GetProperty("PlannedHours");
                hoursProp?.SetValue(activity, dto.PlannedHours.Value);
            }

            if (dto.ResourceRate.HasValue)
            {
                activity.SetResourceRate(dto.ResourceRate.Value);
            }

            await _activityRepository.UpdateAsync(activity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update activity: {ex.Message}");
        }
    }

    public async Task<Result> DeleteActivityAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var activity = await _activityRepository.GetByIdAsync(id, cancellationToken);
            if (activity == null)
                return Result.Failure("Activity not found");

            if (activity.Status == ActivityStatus.Completed)
                return Result.Failure("Cannot delete completed activities");

            // Check for dependencies
            if (!string.IsNullOrWhiteSpace(activity.SuccessorActivities))
            {
                var successors = JsonSerializer.Deserialize<string[]>(activity.SuccessorActivities);
                if (successors?.Any() == true)
                    return Result.Failure("Cannot delete activity with successor dependencies");
            }

            await _activityRepository.DeleteAsync(activity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete activity: {ex.Message}");
        }
    }

    public async Task<Result> UpdateActivityProgressAsync(Guid id, UpdateActivityProgressDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var activity = await _activityRepository.GetByIdAsync(id, cancellationToken);
            if (activity == null)
                return Result.Failure("Activity not found");

            activity.UpdateProgress(dto.ProgressPercentage, dto.ActualHours);

            if (!string.IsNullOrWhiteSpace(dto.Notes))
            {
                // Could store progress notes in a separate table or field
            }

            await _activityRepository.UpdateAsync(activity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update activity progress: {ex.Message}");
        }
    }

    public async Task<Result> StartActivityAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var activity = await _activityRepository.GetByIdAsync(id, cancellationToken);
            if (activity == null)
                return Result.Failure("Activity not found");

            activity.Start();

            await _activityRepository.UpdateAsync(activity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to start activity: {ex.Message}");
        }
    }

    public async Task<Result> CompleteActivityAsync(Guid id, DateTime? actualEndDate, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var activity = await _activityRepository.GetByIdAsync(id, cancellationToken);
            if (activity == null)
                return Result.Failure("Activity not found");

            if (actualEndDate.HasValue)
            {
                var endDateProp = typeof(Activity).GetProperty("ActualEndDate");
                endDateProp?.SetValue(activity, actualEndDate.Value);
            }

            activity.Complete();

            await _activityRepository.UpdateAsync(activity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to complete activity: {ex.Message}");
        }
    }

    public async Task<Result> SuspendActivityAsync(Guid id, string reason, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var activity = await _activityRepository.GetByIdAsync(id, cancellationToken);
            if (activity == null)
                return Result.Failure("Activity not found");

            activity.Suspend();

            // Could store suspension reason in a separate table or field

            await _activityRepository.UpdateAsync(activity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to suspend activity: {ex.Message}");
        }
    }

    public async Task<Result> ResumeActivityAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var activity = await _activityRepository.GetByIdAsync(id, cancellationToken);
            if (activity == null)
                return Result.Failure("Activity not found");

            activity.Resume();

            await _activityRepository.UpdateAsync(activity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to resume activity: {ex.Message}");
        }
    }

    public async Task<Result> CancelActivityAsync(Guid id, string reason, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var activity = await _activityRepository.GetByIdAsync(id, cancellationToken);
            if (activity == null)
                return Result.Failure("Activity not found");

            activity.Cancel();

            // Could store cancellation reason in a separate table or field

            await _activityRepository.UpdateAsync(activity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to cancel activity: {ex.Message}");
        }
    }

    public async Task<Result> BulkUpdateProgressAsync(BulkUpdateActivitiesDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var activities = await _activityRepository.GetAllQueryable()
                .Where(a => dto.ActivityIds.Contains(a.Id))
                .ToListAsync(cancellationToken);

            if (activities.Count != dto.ActivityIds.Count)
                return Result.Failure("One or more activities not found");

            foreach (var activity in activities)
            {
                var update = dto.Updates.FirstOrDefault(u => u.ActivityId == activity.Id);
                if (update != null)
                {
                    activity.UpdateProgress(update.ProgressPercentage, update.ActualHours);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to bulk update activities: {ex.Message}");
        }
    }

    public async Task<Core.Results.Result<int>> BulkCreateActivitiesAsync(List<CreateActivityDto> activities, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var createdActivities = new List<Activity>();

            foreach (var dto in activities)
            {
                // Validate WBS element exists
                var wbsElement = await _wbsRepository.GetByIdAsync(dto.WBSElementId, cancellationToken);
                if (wbsElement == null)
                    continue;

                // Validate activity code is unique
                var codeExists = await _activityRepository.GetAllQueryable()
                    .Include(a => a.WBSElement)
                    .AnyAsync(a => a.WBSElement.ProjectId == wbsElement.ProjectId && a.ActivityCode == dto.ActivityCode, cancellationToken);

                if (codeExists)
                    continue;

                var activity = new Activity(
                    dto.WBSElementId,
                    dto.ActivityCode,
                    dto.Name,
                    dto.PlannedStartDate,
                    dto.PlannedEndDate,
                    dto.PlannedHours);

                createdActivities.Add(activity);
            }

            if (!createdActivities.Any())
                return Result<int>.Failure("No valid activities to create");

            await _activityRepository.AddRangeAsync(createdActivities, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(createdActivities.Count);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Failed to bulk create activities: {ex.Message}");
        }
    }

    public async Task<Result> UpdateActivityScheduleAsync(Guid id, DateTime plannedStart, DateTime plannedEnd, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var activity = await _activityRepository.GetByIdAsync(id, cancellationToken);
            if (activity == null)
                return Result.Failure("Activity not found");

            activity.UpdateSchedule(plannedStart, plannedEnd);

            await _activityRepository.UpdateAsync(activity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update activity schedule: {ex.Message}");
        }
    }

    public async Task<Result> SetActivityDependenciesAsync(Guid id, string[]? predecessors, string[]? successors, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var activity = await _activityRepository.GetByIdAsync(id, cancellationToken);
            if (activity == null)
                return Result.Failure("Activity not found");

            // Validate dependencies exist
            var validationErrors = await ValidateDependenciesAsync(id, predecessors ?? Array.Empty<string>(), successors ?? Array.Empty<string>(), cancellationToken);
            if (validationErrors.Any())
                return Result.Failure($"Invalid dependencies: {string.Join(", ", validationErrors)}");

            activity.SetDependencies(predecessors, successors);

            await _activityRepository.UpdateAsync(activity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to set activity dependencies: {ex.Message}");
        }
    }

    public async Task<Result> RecalculateCriticalPathAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            // This is a placeholder - real critical path calculation would be much more complex
            var activities = await _activityRepository.GetAllQueryable()
                .Include(a => a.WBSElement)
                .Where(a => a.WBSElement.ProjectId == projectId)
                .ToListAsync(cancellationToken);

            // Simplified logic - mark activities with duration > 20 days as critical
            foreach (var activity in activities)
            {
                // In a real implementation, this would use forward/backward pass algorithm
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to recalculate critical path: {ex.Message}");
        }
    }

    public async Task<Result> AssignResourcesAsync(Guid activityId, List<Guid> resourceIds, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var activity = await _activityRepository.GetAllQueryable()
                .Include(a => a.Resources)
                .FirstOrDefaultAsync(a => a.Id == activityId, cancellationToken);

            if (activity == null)
                return Result.Failure("Activity not found");

            // Clear existing resources
            activity.Resources.Clear();

            // Add new resources
            var resources = await _resourceRepository.GetAllQueryable()
                .Where(r => resourceIds.Contains(r.Id))
                .ToListAsync(cancellationToken);

            foreach (var resource in resources)
            {
                activity.Resources.Add(resource);
            }

            await _activityRepository.UpdateAsync(activity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to assign resources: {ex.Message}");
        }
    }

    public async Task<Result> UpdateResourceRateAsync(Guid activityId, decimal rate, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var activity = await _activityRepository.GetByIdAsync(activityId, cancellationToken);
            if (activity == null)
                return Result.Failure("Activity not found");

            activity.SetResourceRate(rate);

            await _activityRepository.UpdateAsync(activity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update resource rate: {ex.Message}");
        }
    }

    public async Task<bool> ValidateActivityCodeAsync(string code, Guid projectId, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _activityRepository.GetAllQueryable()
            .Include(a => a.WBSElement)
            .Where(a => a.WBSElement.ProjectId == projectId && a.ActivityCode == code);

        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> CanDeleteActivityAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var activity = await _activityRepository.GetByIdAsync(id, cancellationToken);
        if (activity == null)
            return false;

        // Cannot delete completed activities
        if (activity.Status == ActivityStatus.Completed)
            return false;

        // Cannot delete if has successors
        if (!string.IsNullOrWhiteSpace(activity.SuccessorActivities))
        {
            var successors = JsonSerializer.Deserialize<string[]>(activity.SuccessorActivities);
            if (successors?.Any() == true)
                return false;
        }

        return true;
    }

    public async Task<List<string>> ValidateDependenciesAsync(Guid activityId, string[] predecessors, string[] successors, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        var activity = await _activityRepository.GetAllQueryable()
            .Include(a => a.WBSElement)
            .FirstOrDefaultAsync(a => a.Id == activityId, cancellationToken);

        if (activity == null)
        {
            errors.Add("Activity not found");
            return errors;
        }

        // Check if predecessors exist
        foreach (var pred in predecessors)
        {
            var exists = await _activityRepository.GetAllQueryable()
                .Include(a => a.WBSElement)
                .AnyAsync(a => a.WBSElement.ProjectId == activity.WBSElement.ProjectId && a.ActivityCode == pred, cancellationToken);

            if (!exists)
                errors.Add($"Predecessor activity '{pred}' not found");
        }

        // Check if successors exist
        foreach (var succ in successors)
        {
            var exists = await _activityRepository.GetAllQueryable()
                .Include(a => a.WBSElement)
                .AnyAsync(a => a.WBSElement.ProjectId == activity.WBSElement.ProjectId && a.ActivityCode == succ, cancellationToken);

            if (!exists)
                errors.Add($"Successor activity '{succ}' not found");
        }

        // Check for circular dependencies (simplified)
        if (predecessors.Contains(activity.ActivityCode) || successors.Contains(activity.ActivityCode))
            errors.Add("Activity cannot be its own predecessor or successor");

        return errors;
    }
}