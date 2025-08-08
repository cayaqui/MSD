using Application.Interfaces;
using Application.Interfaces.Progress;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.Progress.Schedules;
using Core.Enums.Progress;
using Domain.Entities.Progress;
using Microsoft.EntityFrameworkCore;
using Result = Core.Results.Result;

namespace Application.Services.Progress;

public class ScheduleService : IScheduleService
{
    private readonly IRepository<ScheduleVersion> _scheduleRepository;
    private readonly IRepository<Activity> _activityRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ScheduleService(
        IRepository<ScheduleVersion> scheduleRepository,
        IRepository<Activity> activityRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _scheduleRepository = scheduleRepository;
        _activityRepository = activityRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<ScheduleVersionDto>> GetScheduleVersionsAsync(ScheduleFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = _scheduleRepository.GetAllQueryable()
            .Include(s => s.Project)
            .AsQueryable();

        if (filter.ProjectId.HasValue)
            query = query.Where(s => s.ProjectId == filter.ProjectId.Value);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            query = query.Where(s => s.Name.Contains(filter.SearchTerm) || 
                                    s.Version.Contains(filter.SearchTerm) ||
                                    (s.Description != null && s.Description.Contains(filter.SearchTerm)));

        if (filter.Status.HasValue)
            query = query.Where(s => s.Status == filter.Status.Value);

        if (filter.IsBaseline.HasValue)
            query = query.Where(s => s.IsBaseline == filter.IsBaseline.Value);

        if (filter.StartDateFrom.HasValue)
            query = query.Where(s => s.PlannedStartDate >= filter.StartDateFrom.Value);

        if (filter.StartDateTo.HasValue)
            query = query.Where(s => s.PlannedStartDate <= filter.StartDateTo.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = filter.SortBy?.ToLower() switch
        {
            "version" => filter.IsDescending ? query.OrderByDescending(s => s.Version) : query.OrderBy(s => s.Version),
            "startdate" => filter.IsDescending ? query.OrderByDescending(s => s.PlannedStartDate) : query.OrderBy(s => s.PlannedStartDate),
            "status" => filter.IsDescending ? query.OrderByDescending(s => s.Status) : query.OrderBy(s => s.Status),
            _ => filter.IsDescending ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt)
        };

        // Apply pagination
        var schedules = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        var scheduleDtos = _mapper.Map<List<ScheduleVersionDto>>(schedules);

        return new PagedResult<ScheduleVersionDto>(
            scheduleDtos,
            totalCount,
            filter.PageNumber,
            filter.PageSize);
    }

    public async Task<ScheduleVersionDto?> GetScheduleVersionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var schedule = await _scheduleRepository.GetAllQueryable()
            .Include(s => s.Project)
            .Include(s => s.Activities)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return schedule == null ? null : _mapper.Map<ScheduleVersionDto>(schedule);
    }

    public async Task<ScheduleVersionDto?> GetActiveScheduleAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var schedule = await _scheduleRepository.GetAllQueryable()
            .Include(s => s.Project)
            .Where(s => s.ProjectId == projectId && s.IsBaseline)
            .OrderByDescending(s => s.BaselineDate)
            .FirstOrDefaultAsync(cancellationToken);

        return schedule == null ? null : _mapper.Map<ScheduleVersionDto>(schedule);
    }

    public async Task<ScheduleVersionDto?> GetCurrentScheduleAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        // Get the most recent active schedule (not necessarily baseline)
        var schedule = await _scheduleRepository.GetAllQueryable()
            .Include(s => s.Project)
            .Where(s => s.ProjectId == projectId && (s.Status == ScheduleStatus.Approved || s.Status == ScheduleStatus.Baselined))
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return schedule == null ? null : _mapper.Map<ScheduleVersionDto>(schedule);
    }

    public async Task<ScheduleVersionDto?> GetBaselineScheduleAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        // Get the baseline schedule
        var schedule = await _scheduleRepository.GetAllQueryable()
            .Include(s => s.Project)
            .Where(s => s.ProjectId == projectId && s.IsBaseline && s.Status == ScheduleStatus.Baselined)
            .FirstOrDefaultAsync(cancellationToken);

        return schedule == null ? null : _mapper.Map<ScheduleVersionDto>(schedule);
    }

    public async Task<List<ScheduleVersionDto>> GetProjectSchedulesAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var schedules = await _scheduleRepository.GetAllQueryable()
            .Include(s => s.Project)
            .Where(s => s.ProjectId == projectId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<ScheduleVersionDto>>(schedules);
    }

    public async Task<Core.Results.Result<Guid>> CreateScheduleVersionAsync(CreateScheduleVersionDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate version doesn't already exist
            var existingVersion = await _scheduleRepository.GetAllQueryable()
                .AnyAsync(s => s.ProjectId == dto.ProjectId && s.Version == dto.Version, cancellationToken);

            if (existingVersion)
                return Result<Guid>.Failure("A schedule with this version already exists for the project");

            var schedule = new ScheduleVersion(
                dto.ProjectId,
                dto.Version,
                dto.Name,
                dto.PlannedStartDate,
                dto.PlannedEndDate);

            if (!string.IsNullOrWhiteSpace(dto.Description))
            {
                var descProp = typeof(ScheduleVersion).GetProperty("Description");
                descProp?.SetValue(schedule, dto.Description);
            }

            if (!string.IsNullOrWhiteSpace(dto.SourceSystem))
            {
                schedule.RecordImport(dto.SourceSystem, userId.ToString());
            }

            await _scheduleRepository.AddAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(schedule.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to create schedule version: {ex.Message}");
        }
    }

    public async Task<Result> UpdateScheduleVersionAsync(Guid id, UpdateScheduleVersionDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id, cancellationToken);
            if (schedule == null)
                return Result.Failure("Schedule version not found");

            // Update basic properties using reflection for private setters
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                var nameProp = typeof(ScheduleVersion).GetProperty("Name");
                nameProp?.SetValue(schedule, dto.Name);
            }

            if (!string.IsNullOrWhiteSpace(dto.Description))
            {
                var descProp = typeof(ScheduleVersion).GetProperty("Description");
                descProp?.SetValue(schedule, dto.Description);
            }

            if (dto.DataDate.HasValue)
            {
                schedule.SetDataDate(dto.DataDate.Value);
            }

            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update schedule version: {ex.Message}");
        }
    }

    public async Task<Result> DeleteScheduleVersionAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id, cancellationToken);
            if (schedule == null)
                return Result.Failure("Schedule version not found");

            if (schedule.IsBaseline)
                return Result.Failure("Cannot delete a baseline schedule");

            if (schedule.Status == ScheduleStatus.Approved || schedule.Status == ScheduleStatus.Baselined)
                return Result.Failure("Cannot delete an approved or baselined schedule");

            await _scheduleRepository.DeleteAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete schedule version: {ex.Message}");
        }
    }

    public async Task<Result> SetAsBaselineAsync(Guid scheduleId, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId, cancellationToken);
            if (schedule == null)
                return Result.Failure("Schedule version not found");

            // Remove baseline status from any existing baseline
            var existingBaseline = await _scheduleRepository.GetAllQueryable()
                .Where(s => s.ProjectId == schedule.ProjectId && s.IsBaseline && s.Id != scheduleId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingBaseline != null)
            {
                var isBaselineProp = typeof(ScheduleVersion).GetProperty("IsBaseline");
                isBaselineProp?.SetValue(existingBaseline, false);
                await _scheduleRepository.UpdateAsync(existingBaseline, cancellationToken);
            }

            schedule.SetAsBaseline();
            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to set schedule as baseline: {ex.Message}");
        }
    }

    public async Task<Result> ApproveScheduleAsync(Guid scheduleId, Guid userId, string? comments = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId, cancellationToken);
            if (schedule == null)
                return Result.Failure("Schedule version not found");

            schedule.Approve(userId.ToString(), comments);
            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to approve schedule: {ex.Message}");
        }
    }

    public async Task<Result> RejectScheduleAsync(Guid scheduleId, Guid userId, string comments, CancellationToken cancellationToken = default)
    {
        try
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId, cancellationToken);
            if (schedule == null)
                return Result.Failure("Schedule version not found");

            if (schedule.Status != ScheduleStatus.UnderReview)
                return Result.Failure("Only schedules under review can be rejected");

            // Set status back to Draft
            var statusProp = typeof(ScheduleVersion).GetProperty("Status");
            statusProp?.SetValue(schedule, ScheduleStatus.Draft);

            var commentsProp = typeof(ScheduleVersion).GetProperty("ApprovalComments");
            commentsProp?.SetValue(schedule, comments);

            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to reject schedule: {ex.Message}");
        }
    }

    public async Task<Result> ArchiveScheduleAsync(Guid scheduleId, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId, cancellationToken);
            if (schedule == null)
                return Result.Failure("Schedule version not found");

            if (schedule.IsBaseline)
                return Result.Failure("Cannot archive a baseline schedule");

            var statusProp = typeof(ScheduleVersion).GetProperty("Status");
            statusProp?.SetValue(schedule, ScheduleStatus.Archived);

            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to archive schedule: {ex.Message}");
        }
    }

    public async Task<Core.Results.Result<ScheduleHealthDto>> GetScheduleHealthAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var schedule = await _scheduleRepository.GetAllQueryable()
                .Include(s => s.Activities)
                .FirstOrDefaultAsync(s => s.Id == scheduleId, cancellationToken);

            if (schedule == null)
                return Result<ScheduleHealthDto>.Failure("Schedule version not found");

            var activities = schedule.Activities;
            var totalActivities = activities.Count;
            var completedActivities = activities.Count(a => a.Status == Core.Enums.Progress.ActivityStatus.Completed);
            var inProgressActivities = activities.Count(a => a.Status == Core.Enums.Progress.ActivityStatus.InProgress);
            var delayedActivities = activities.Count(a => a.ActualEndDate > a.PlannedEndDate);

            var healthDto = new ScheduleHealthDto
            {
                ScheduleId = schedule.Id,
                TotalActivities = totalActivities,
                CompletedActivities = completedActivities,
                InProgressActivities = inProgressActivities,
                DelayedActivities = delayedActivities,
                OnTrackActivities = totalActivities - delayedActivities,
                OverallProgress = totalActivities > 0 ? (decimal)completedActivities / totalActivities * 100 : 0,
                SchedulePerformanceIndex = CalculateSPI(activities),
                CriticalPathHealth = schedule.CriticalActivities > 0 ? 
                    (decimal)(schedule.CriticalActivities - activities.Count(a => a.Status == Core.Enums.Progress.ActivityStatus.Delayed)) / schedule.CriticalActivities * 100 : 100,
                LastUpdated = schedule.UpdatedAt
            };

            return Result<ScheduleHealthDto>.Success(healthDto);
        }
        catch (Exception ex)
        {
            return Result<ScheduleHealthDto>.Failure($"Failed to get schedule health: {ex.Message}");
        }
    }

    public async Task<Core.Results.Result<CriticalPathAnalysisDto>> GetCriticalPathAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var schedule = await _scheduleRepository.GetAllQueryable()
                .Include(s => s.Activities)
                .FirstOrDefaultAsync(s => s.Id == scheduleId, cancellationToken);

            if (schedule == null)
                return Result<CriticalPathAnalysisDto>.Failure("Schedule version not found");

            // This is a simplified version - real critical path calculation would be more complex
            var criticalActivities = schedule.Activities
                .Where(a => a.ProgressPercentage < 100)
                .OrderBy(a => a.PlannedStartDate)
                .Take(10) // Simplified - take first 10 activities
                .ToList();

            var analysisDto = new CriticalPathAnalysisDto
            {
                ScheduleId = schedule.Id,
                CriticalPathLength = schedule.TotalDuration,
                CriticalActivities = _mapper.Map<List<CriticalActivityDto>>(criticalActivities),
                TotalFloat = schedule.TotalFloat,
                ProjectCompletionDate = schedule.PlannedEndDate,
                ForecastCompletionDate = CalculateForecastCompletion(schedule, criticalActivities),
                CriticalPathStatus = criticalActivities.Any(a => a.Status == Core.Enums.Progress.ActivityStatus.Delayed) ? "At Risk" : "On Track"
            };

            return Result<CriticalPathAnalysisDto>.Success(analysisDto);
        }
        catch (Exception ex)
        {
            return Result<CriticalPathAnalysisDto>.Failure($"Failed to get critical path: {ex.Message}");
        }
    }

    public async Task<Core.Results.Result<ScheduleComparisonDto>> CompareSchedulesAsync(Guid baselineId, Guid currentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var baseline = await _scheduleRepository.GetAllQueryable()
                .Include(s => s.Activities)
                .FirstOrDefaultAsync(s => s.Id == baselineId, cancellationToken);

            var current = await _scheduleRepository.GetAllQueryable()
                .Include(s => s.Activities)
                .FirstOrDefaultAsync(s => s.Id == currentId, cancellationToken);

            if (baseline == null || current == null)
                return Result<ScheduleComparisonDto>.Failure("One or both schedules not found");

            var comparisonDto = new ScheduleComparisonDto
            {
                BaselineScheduleId = baselineId,
                CurrentScheduleId = currentId,
                BaselineStartDate = baseline.PlannedStartDate,
                CurrentStartDate = current.PlannedStartDate,
                BaselineEndDate = baseline.PlannedEndDate,
                CurrentEndDate = current.PlannedEndDate,
                StartVarianceDays = (current.PlannedStartDate - baseline.PlannedStartDate).Days,
                EndVarianceDays = (current.PlannedEndDate - baseline.PlannedEndDate).Days,
                BaselineDuration = baseline.TotalDuration,
                CurrentDuration = current.TotalDuration,
                DurationVarianceDays = current.TotalDuration - baseline.TotalDuration,
                AddedActivities = current.Activities.Count - baseline.Activities.Count,
                RemovedActivities = 0, // Would need more complex logic
                ModifiedActivities = 0, // Would need more complex logic
                OverallVariancePercentage = baseline.TotalDuration > 0 ? 
                    (decimal)(current.TotalDuration - baseline.TotalDuration) / baseline.TotalDuration * 100 : 0
            };

            return Result<ScheduleComparisonDto>.Success(comparisonDto);
        }
        catch (Exception ex)
        {
            return Result<ScheduleComparisonDto>.Failure($"Failed to compare schedules: {ex.Message}");
        }
    }

    public async Task<Core.Results.Result<List<ScheduleVarianceDto>>> GetScheduleVariancesAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var schedule = await _scheduleRepository.GetAllQueryable()
                .Include(s => s.Activities)
                .FirstOrDefaultAsync(s => s.Id == scheduleId, cancellationToken);

            if (schedule == null)
                return Result<List<ScheduleVarianceDto>>.Failure("Schedule version not found");

            var variances = schedule.Activities
                .Where(a => a.ActualStartDate.HasValue || a.ActualEndDate.HasValue)
                .Select(a => new ScheduleVarianceDto
                {
                    ActivityId = a.Id,
                    ActivityCode = a.ActivityCode,
                    ActivityName = a.Name,
                    PlannedStart = a.PlannedStartDate,
                    ActualStart = a.ActualStartDate,
                    StartVariance = a.ActualStartDate.HasValue ? 
                        (a.ActualStartDate.Value - a.PlannedStartDate).Days : 0,
                    PlannedEnd = a.PlannedEndDate,
                    ActualEnd = a.ActualEndDate,
                    EndVariance = a.ActualEndDate.HasValue ? 
                        (a.ActualEndDate.Value - a.PlannedEndDate).Days : 0,
                    VarianceType = DetermineVarianceType(a),
                    ImpactLevel = DetermineImpactLevel(a)
                })
                .OrderByDescending(v => Math.Abs(v.EndVariance))
                .ToList();

            return Result<List<ScheduleVarianceDto>>.Success(variances);
        }
        catch (Exception ex)
        {
            return Result<List<ScheduleVarianceDto>>.Failure($"Failed to get schedule variances: {ex.Message}");
        }
    }

    public async Task<Result> ValidateScheduleIntegrityAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var schedule = await _scheduleRepository.GetAllQueryable()
                .Include(s => s.Activities)
                .FirstOrDefaultAsync(s => s.Id == scheduleId, cancellationToken);

            if (schedule == null)
                return Result.Failure("Schedule version not found");

            var errors = new List<string>();

            // Check for activities without dates
            if (schedule.Activities.Any(a => a.PlannedStartDate == default || a.PlannedEndDate == default))
                errors.Add("Some activities have missing planned dates");

            // Check for invalid date sequences
            if (schedule.Activities.Any(a => a.PlannedEndDate < a.PlannedStartDate))
                errors.Add("Some activities have end dates before start dates");

            // Check for orphaned activities (simplified check)
            if (schedule.Activities.Any(a => a.WBSElementId == Guid.Empty))
                errors.Add("Some activities are not assigned to WBS elements");

            if (errors.Any())
                return Result.Failure($"Schedule validation failed: {string.Join(", ", errors)}");

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to validate schedule: {ex.Message}");
        }
    }

    public async Task<Result> RecalculateScheduleAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var schedule = await _scheduleRepository.GetAllQueryable()
                .Include(s => s.Activities)
                .FirstOrDefaultAsync(s => s.Id == scheduleId, cancellationToken);

            if (schedule == null)
                return Result.Failure("Schedule version not found");

            // Recalculate statistics
            var totalActivities = schedule.Activities.Count;
            var criticalActivities = schedule.Activities.Count(a => a.DurationDays > 10); // Simplified
            var totalFloat = schedule.Activities.Sum(a => (decimal)a.DurationDays * 0.1m); // Simplified

            schedule.UpdateStatistics(totalActivities, criticalActivities, totalFloat);

            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to recalculate schedule: {ex.Message}");
        }
    }

    public async Task<bool> CanModifyScheduleAsync(Guid scheduleId, Guid userId, CancellationToken cancellationToken = default)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(scheduleId, cancellationToken);
        if (schedule == null)
            return false;

        // Only draft schedules can be modified
        return schedule.Status == ScheduleStatus.Draft;
    }

    public async Task<bool> CanApproveScheduleAsync(Guid scheduleId, Guid userId, CancellationToken cancellationToken = default)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(scheduleId, cancellationToken);
        if (schedule == null)
            return false;

        // Only schedules under review can be approved
        return schedule.Status == ScheduleStatus.UnderReview;
    }

    public async Task<Result> SubmitForApprovalAsync(Guid scheduleId, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId, cancellationToken);
            if (schedule == null)
                return Result.Failure("Schedule version not found");

            schedule.Submit(userId.ToString());
            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to submit schedule for approval: {ex.Message}");
        }
    }

    public async Task<Result> ApproveScheduleAsync(Guid scheduleId, ApproveScheduleDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId, cancellationToken);
            if (schedule == null)
                return Result.Failure("Schedule version not found");

            schedule.Approve(userId.ToString(), dto.Comments);
            
            if (dto.SetAsBaseline)
            {
                schedule.SetAsBaseline();
            }
            
            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to approve schedule: {ex.Message}");
        }
    }

    public async Task<List<ScheduleVarianceDto>> GetProjectScheduleVariancesAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var baseline = await GetBaselineScheduleAsync(projectId, cancellationToken);
        var current = await GetCurrentScheduleAsync(projectId, cancellationToken);

        if (baseline == null || current == null)
            return new List<ScheduleVarianceDto>();

        // Compare baseline and current schedules
        var comparisonResult = await CompareSchedulesAsync(baseline.Id, current.Id, cancellationToken);
        if (!comparisonResult.IsSuccess)
            return new List<ScheduleVarianceDto>();

        // Get variances for the current schedule
        var variancesResult = await GetScheduleVariancesAsync(current.Id, cancellationToken);
        return variancesResult.IsSuccess ? variancesResult.Value : new List<ScheduleVarianceDto>();
    }

    public async Task<Core.Results.Result<Guid>> ImportScheduleAsync(ImportScheduleDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Create new schedule version
            var schedule = new ScheduleVersion(
                dto.ProjectId,
                dto.Version,
                dto.Name,
                DateTime.UtcNow, // Will be updated from import
                DateTime.UtcNow.AddDays(90)); // Will be updated from import

            schedule.RecordImport(dto.SourceSystem, userId.ToString());

            // TODO: Parse the file content based on source system
            // This would require integration with MS Project or Primavera file parsers
            // For now, just create the schedule

            await _scheduleRepository.AddAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(schedule.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to import schedule: {ex.Message}");
        }
    }

    public async Task<byte[]> ExportScheduleAsync(Guid scheduleId, string format, CancellationToken cancellationToken = default)
    {
        var schedule = await _scheduleRepository.GetAllQueryable()
            .Include(s => s.Activities)
            .FirstOrDefaultAsync(s => s.Id == scheduleId, cancellationToken);

        if (schedule == null)
            throw new InvalidOperationException("Schedule not found");

        // TODO: Implement export logic based on format
        // This would require integration with MS Project or Primavera file generators
        // For now, return empty byte array
        return Array.Empty<byte>();
    }

    public async Task<byte[]> ExportScheduleTemplateAsync(string format, CancellationToken cancellationToken = default)
    {
        // TODO: Implement template export logic based on format
        // This would provide a template file for importing schedules
        // For now, return empty byte array
        return Array.Empty<byte>();
    }

    public async Task<bool> CanCreateNewVersionAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        // Check if there's already a draft version
        var hasDraft = await _scheduleRepository.GetAllQueryable()
            .AnyAsync(s => s.ProjectId == projectId && s.Status == ScheduleStatus.Draft, cancellationToken);

        return !hasDraft;
    }

    public async Task<bool> CanSetAsBaselineAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(scheduleId, cancellationToken);
        if (schedule == null)
            return false;

        // Only approved schedules can be set as baseline
        return schedule.Status == ScheduleStatus.Approved;
    }

    // Helper methods
    private decimal CalculateSPI(IEnumerable<Activity> activities)
    {
        var plannedValue = activities.Sum(a => a.PlannedHours * (a.ProgressPercentage / 100));
        var earnedValue = activities.Sum(a => a.ActualHours);
        
        return earnedValue > 0 ? plannedValue / earnedValue : 1;
    }

    private DateTime CalculateForecastCompletion(ScheduleVersion schedule, List<Activity> criticalActivities)
    {
        if (!criticalActivities.Any())
            return schedule.PlannedEndDate;

        var avgDelay = criticalActivities
            .Where(a => a.ActualEndDate.HasValue && a.ActualEndDate > a.PlannedEndDate)
            .Select(a => (a.ActualEndDate!.Value - a.PlannedEndDate).Days)
            .DefaultIfEmpty(0)
            .Average();

        return schedule.PlannedEndDate.AddDays(avgDelay);
    }

    private string DetermineVarianceType(Activity activity)
    {
        if (activity.ActualEndDate.HasValue && activity.ActualEndDate > activity.PlannedEndDate)
            return "Delayed";
        if (activity.ActualEndDate.HasValue && activity.ActualEndDate < activity.PlannedEndDate)
            return "Early";
        return "On Schedule";
    }

    private string DetermineImpactLevel(Activity activity)
    {
        if (!activity.ActualEndDate.HasValue)
            return "Unknown";

        var variance = (activity.ActualEndDate.Value - activity.PlannedEndDate).Days;
        return Math.Abs(variance) switch
        {
            > 30 => "High",
            > 7 => "Medium",
            _ => "Low"
        };
    }
}