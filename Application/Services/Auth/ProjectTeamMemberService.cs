using Core.Constants;
using Core.DTOs.Auth.ProjectTeamMembers;


namespace Application.Services.Auth;

public class ProjectTeamMemberService : IProjectTeamMemberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProjectTeamMemberService> _logger;
    private readonly ICurrentUserService _currentUserService;

    public ProjectTeamMemberService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ProjectTeamMemberService> logger,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<ProjectTeamMemberDetailDto>> GetPagedAsync(ProjectTeamMemberFilterDto filter)
    {
        var query = _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .Include(ptm => ptm.User)
            .Include(ptm => ptm.Project)
            .AsQueryable();

        // Apply filters
        query = ApplyFilters(query, filter);

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply ordering
        query = query.OrderBy(ptm => ptm.Project.Name)
                    .ThenBy(ptm => ptm.User.Name);

        // Apply pagination
        var members = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var memberDtos = members.Select(ptm => MapToDetailDto(ptm)).ToList();

        return new PagedResult<ProjectTeamMemberDetailDto>
        {
            Items = memberDtos,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<ProjectTeamMemberDetailDto?> GetByIdAsync(Guid id)
    {
        var member = await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .Include(ptm => ptm.User)
            .Include(ptm => ptm.Project)
            .FirstOrDefaultAsync(ptm => ptm.Id == id);

        return member != null ? MapToDetailDto(member) : null;
    }

    public async Task<IEnumerable<ProjectTeamMemberDetailDto>> GetByProjectAsync(Guid projectId)
    {
        var members = await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .Include(ptm => ptm.User)
            .Include(ptm => ptm.Project)
            .Where(ptm => ptm.ProjectId == projectId && ptm.IsActive)
            .OrderBy(ptm => ptm.User.Name)
            .ToListAsync();

        return members.Select(MapToDetailDto);
    }

    public async Task<IEnumerable<ProjectTeamMemberDetailDto>> GetByUserAsync(Guid userId)
    {
        var members = await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .Include(ptm => ptm.User)
            .Include(ptm => ptm.Project)
            .Where(ptm => ptm.UserId == userId && ptm.IsActive)
            .OrderBy(ptm => ptm.Project.Name)
            .ToListAsync();

        return members.Select(MapToDetailDto);
    }

    public async Task<ProjectTeamMemberDetailDto> CreateAsync(Guid projectId, AssignProjectTeamMemberDto dto)
    {
        // Verify user has permission to manage team
        if (!await _currentUserService.HasProjectAccessAsync(projectId, ProjectRoles.ProjectManager))
        {
            throw new ForbiddenAccessException();
        }

        // Verify project exists
        var project = await _unitOfWork.Repository<Project>()
            .Query()
            .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted);

        if (project == null)
            throw new NotFoundException(nameof(Project), projectId);

        // Verify user exists
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .FirstOrDefaultAsync(u => u.Id == dto.UserId && !u.IsDeleted && u.IsActive);

        if (user == null)
            throw new NotFoundException(nameof(User), dto.UserId);

        // Check if user is already assigned
        var existingMember = await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .FirstOrDefaultAsync(ptm => ptm.ProjectId == projectId && ptm.UserId == dto.UserId);

        if (existingMember != null)
        {
            if (existingMember.IsActive)
                throw new BadRequestException("User is already assigned to this project");

            // Reactivate if previously removed
            existingMember.IsActive = true;
            existingMember.Role = dto.Role;
            existingMember.AllocationPercentage = dto.AllocationPercentage;
            existingMember.StartDate = dto.StartDate;
            existingMember.EndDate = dto.EndDate;
            existingMember.UpdatedAt = DateTime.UtcNow;
            existingMember.UpdatedBy = _currentUserService.UserId;

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Reactivated team member {UserId} in project {ProjectId}", dto.UserId, projectId);

            return await GetByIdAsync(existingMember.Id) ?? throw new InvalidOperationException();
        }

        // Validate allocation
        var currentAllocation = await GetUserTotalAllocationAsync(dto.UserId, dto.StartDate);
        if (currentAllocation + dto.AllocationPercentage > 100)
        {
            throw new BadRequestException($"User allocation would exceed 100%. Current allocation: {currentAllocation}%");
        }

        // Create new member
        var teamMember = new ProjectTeamMember(
            projectId,
            dto.UserId,
            dto.Role,
            dto.StartDate)
        {
            AllocationPercentage = dto.AllocationPercentage,
            EndDate = dto.EndDate,
            CreatedBy = _currentUserService.UserId
        };

        await _unitOfWork.Repository<ProjectTeamMember>().AddAsync(teamMember);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Assigned user {UserId} to project {ProjectId} as {Role}",
            dto.UserId, projectId, dto.Role);

        return await GetByIdAsync(teamMember.Id) ?? throw new InvalidOperationException();
    }

    public async Task<ProjectTeamMemberDetailDto?> UpdateAsync(Guid id, UpdateProjectTeamMemberDto dto)
    {
        var member = await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .Include(ptm => ptm.User)
            .Include(ptm => ptm.Project)
            .FirstOrDefaultAsync(ptm => ptm.Id == id);

        if (member == null)
            return null;

        // Verify user has permission
        if (!await _currentUserService.HasProjectAccessAsync(member.ProjectId, ProjectRoles.ProjectManager))
        {
            throw new ForbiddenAccessException();
        }

        // Don't allow changing the last Project Manager
        if (member.Role == ProjectRoles.ProjectManager && dto.Role != ProjectRoles.ProjectManager)
        {
            var otherPMs = await _unitOfWork.Repository<ProjectTeamMember>()
                .Query()
                .CountAsync(ptm =>
                    ptm.ProjectId == member.ProjectId &&
                    ptm.Id != id &&
                    ptm.Role == ProjectRoles.ProjectManager &&
                    ptm.IsActive);

            if (otherPMs == 0)
                throw new BadRequestException("Cannot change role of the last Project Manager");
        }

        // Update fields
        if (!string.IsNullOrEmpty(dto.Role))
            member.Role = dto.Role;

        if (dto.AllocationPercentage.HasValue)
        {
            // Validate allocation
            var currentAllocation = await GetUserTotalAllocationAsync(
                member.UserId,
                dto.StartDate ?? member.StartDate,
                id); // Exclude current assignment

            if (currentAllocation + dto.AllocationPercentage.Value > 100)
            {
                throw new BadRequestException($"User allocation would exceed 100%. Current allocation: {currentAllocation}%");
            }

            member.AllocationPercentage = dto.AllocationPercentage.Value;
        }

        if (dto.StartDate.HasValue)
            member.StartDate = dto.StartDate.Value;

        if (dto.EndDate.HasValue)
            member.EndDate = dto.EndDate.Value;

        member.UpdatedAt = DateTime.UtcNow;
        member.UpdatedBy = _currentUserService.UserId;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Updated team member {MemberId}", id);

        return MapToDetailDto(member);
    }

    public async Task RemoveAsync(Guid id)
    {
        var member = await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .FirstOrDefaultAsync(ptm => ptm.Id == id);

        if (member == null)
            throw new NotFoundException(nameof(ProjectTeamMember), id);

        // Verify user has permission
        if (!await _currentUserService.HasProjectAccessAsync(member.ProjectId, ProjectRoles.ProjectManager))
        {
            throw new ForbiddenAccessException();
        }

        // Don't allow removing the last Project Manager
        if (member.Role == ProjectRoles.ProjectManager)
        {
            var otherPMs = await _unitOfWork.Repository<ProjectTeamMember>()
                .Query()
                .CountAsync(ptm =>
                    ptm.ProjectId == member.ProjectId &&
                    ptm.Id != id &&
                    ptm.Role == ProjectRoles.ProjectManager &&
                    ptm.IsActive);

            if (otherPMs == 0)
                throw new BadRequestException("Cannot remove the last Project Manager");
        }

        member.IsActive = false;
        member.EndDate = DateTime.UtcNow;
        member.UpdatedAt = DateTime.UtcNow;
        member.UpdatedBy = _currentUserService.UserId;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Removed team member {MemberId}", id);
    }

    public async Task<int> BulkAssignAsync(BulkAssignProjectTeamDto dto)
    {
        // Verify user has permission
        if (!await _currentUserService.HasProjectAccessAsync(dto.ProjectId, ProjectRoles.ProjectManager))
        {
            throw new ForbiddenAccessException();
        }

        var assignedCount = 0;

        foreach (var assignment in dto.Assignments)
        {
            try
            {
                assignment.ProjectId = dto.ProjectId; // Ensure project ID is set
                await CreateAsync(dto.ProjectId, assignment);
                assignedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to assign user {UserId} to project {ProjectId}",
                    assignment.UserId, dto.ProjectId);
            }
        }

        _logger.LogInformation("Bulk assigned {Count} users to project {ProjectId}",
            assignedCount, dto.ProjectId);

        return assignedCount;
    }

    public async Task<int> RemoveAllFromProjectAsync(Guid projectId)
    {
        // Verify user has permission
        if (!await _currentUserService.HasProjectAccessAsync(projectId, ProjectRoles.ProjectManager))
        {
            throw new ForbiddenAccessException();
        }

        var members = await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .Where(ptm => ptm.ProjectId == projectId && ptm.IsActive)
            .ToListAsync();

        // Ensure at least one Project Manager remains
        var projectManagers = members.Where(m => m.Role == ProjectRoles.ProjectManager).ToList();
        if (projectManagers.Count == members.Count && projectManagers.Any())
        {
            throw new BadRequestException("Cannot remove all team members. At least one Project Manager must remain");
        }

        var removedCount = 0;
        foreach (var member in members)
        {
            // Keep at least one PM
            if (member.Role == ProjectRoles.ProjectManager && projectManagers.Count == 1)
                continue;

            member.IsActive = false;
            member.EndDate = DateTime.UtcNow;
            member.UpdatedAt = DateTime.UtcNow;
            member.UpdatedBy = _currentUserService.UserId;
            removedCount++;
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Removed {Count} team members from project {ProjectId}",
            removedCount, projectId);

        return removedCount;
    }

    public async Task<UserAvailabilityDto?> GetUserAvailabilityAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

        if (user == null)
            return null;

        var checkDate = startDate ?? DateTime.UtcNow;
        var checkEndDate = endDate ?? checkDate.AddMonths(3);

        var assignments = await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .Include(ptm => ptm.Project)
            .Where(ptm => ptm.UserId == userId &&
                         ptm.IsActive &&
                         ptm.StartDate <= checkEndDate &&
                         (!ptm.EndDate.HasValue || ptm.EndDate.Value >= checkDate))
            .ToListAsync();

        var totalAllocation = assignments
            .Where(a => a.StartDate <= checkDate && (!a.EndDate.HasValue || a.EndDate.Value >= checkDate))
            .Sum(a => a.AllocationPercentage ?? 0);

        var projectAllocations = assignments.Select(a => new ProjectAllocationDto
        {
            ProjectId = a.ProjectId,
            ProjectName = a.Project.Name,
            ProjectCode = a.Project.Code,
            Role = a.Role,
            AllocationPercentage = a.AllocationPercentage ?? 0,
            StartDate = a.StartDate,
            EndDate = a.EndDate
        }).ToList();

        return new UserAvailabilityDto
        {
            UserId = userId,
            UserName = user.Name,
            TotalAllocation = totalAllocation,
            AvailableCapacity = Math.Max(0, 100 - totalAllocation),
            ProjectAllocations = projectAllocations
        };
    }

    public async Task<IEnumerable<TeamAllocationReportDto>> GetAllocationReportAsync(
        DateTime? date = null,
        Guid? projectId = null,
        Guid? userId = null)
    {
        var checkDate = date ?? DateTime.UtcNow;

        var query = _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .Include(ptm => ptm.User)
            .Include(ptm => ptm.Project)
            .Where(ptm => ptm.IsActive &&
                         ptm.StartDate <= checkDate &&
                         (!ptm.EndDate.HasValue || ptm.EndDate.Value >= checkDate));

        if (projectId.HasValue)
            query = query.Where(ptm => ptm.ProjectId == projectId.Value);

        if (userId.HasValue)
            query = query.Where(ptm => ptm.UserId == userId.Value);

        var assignments = await query.ToListAsync();

        var report = assignments
            .GroupBy(a => new { a.UserId, a.User.Name, a.User.Email })
            .Select(g => new TeamAllocationReportDto
            {
                UserId = g.Key.UserId,
                UserName = g.Key.Name,
                UserEmail = g.Key.Email,
                TotalAllocation = g.Sum(a => a.AllocationPercentage ?? 0),
                ProjectCount = g.Count(),
                IsOverAllocated = g.Sum(a => a.AllocationPercentage ?? 0) > 100,
                Projects = g.Select(a => new ProjectAllocationDto
                {
                    ProjectId = a.ProjectId,
                    ProjectName = a.Project.Name,
                    ProjectCode = a.Project.Code,
                    Role = a.Role,
                    AllocationPercentage = a.AllocationPercentage ?? 0,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate
                }).ToList()
            })
            .OrderBy(r => r.UserName)
            .ToList();

        return report;
    }

    public async Task<ProjectTeamMemberDetailDto?> UpdateAllocationAsync(Guid id, decimal allocationPercentage)
    {
        var dto = new UpdateProjectTeamMemberDto
        {
            AllocationPercentage = allocationPercentage
        };

        return await UpdateAsync(id, dto);
    }

    public async Task<ProjectTeamMemberDetailDto?> TransferAsync(Guid id, TransferTeamMemberDto dto)
    {
        var member = await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .Include(ptm => ptm.User)
            .Include(ptm => ptm.Project)
            .FirstOrDefaultAsync(ptm => ptm.Id == id);

        if (member == null)
            return null;

        // Verify permissions on both projects
        if (!await _currentUserService.HasProjectAccessAsync(member.ProjectId, ProjectRoles.ProjectManager) ||
            !await _currentUserService.HasProjectAccessAsync(dto.NewProjectId, ProjectRoles.ProjectManager))
        {
            throw new ForbiddenAccessException();
        }

        // End current assignment
        member.IsActive = false;
        member.EndDate = dto.TransferDate.AddDays(-1);
        member.UpdatedAt = DateTime.UtcNow;
        member.UpdatedBy = _currentUserService.UserId;

        // Create new assignment
        var newAssignment = new ProjectTeamMember(
            dto.NewProjectId,
            member.UserId,
            dto.NewRole ?? member.Role,
            dto.TransferDate)
        {
            AllocationPercentage = member.AllocationPercentage,
            CreatedBy = _currentUserService.UserId
        };

        await _unitOfWork.Repository<ProjectTeamMember>().AddAsync(newAssignment);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Transferred team member {UserId} from project {OldProjectId} to {NewProjectId}",
            member.UserId, member.ProjectId, dto.NewProjectId);

        return await GetByIdAsync(newAssignment.Id);
    }

    public async Task<ProjectTeamMemberDetailDto?> ExtendAssignmentAsync(Guid id, DateTime newEndDate)
    {
        var member = await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .Include(ptm => ptm.User)
            .Include(ptm => ptm.Project)
            .FirstOrDefaultAsync(ptm => ptm.Id == id);

        if (member == null)
            return null;

        // Verify permission
        if (!await _currentUserService.HasProjectAccessAsync(member.ProjectId, ProjectRoles.ProjectManager))
        {
            throw new ForbiddenAccessException();
        }

        if (newEndDate <= member.StartDate)
            throw new BadRequestException("New end date must be after the start date");

        member.EndDate = newEndDate;
        member.UpdatedAt = DateTime.UtcNow;
        member.UpdatedBy = _currentUserService.UserId;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Extended assignment {MemberId} to {NewEndDate}", id, newEndDate);

        return MapToDetailDto(member);
    }

    public async Task<bool> IsUserAssignedToProjectAsync(Guid userId, Guid projectId)
    {
        return await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .AnyAsync(ptm => ptm.UserId == userId &&
                            ptm.ProjectId == projectId &&
                            ptm.IsActive);
    }

    public async Task<bool> CanUserBeAssignedAsync(Guid userId, Guid projectId, decimal? allocationPercentage = null)
    {
        // Check if user exists and is active
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted && u.IsActive);

        if (user == null)
            return false;

        // Check if already assigned
        if (await IsUserAssignedToProjectAsync(userId, projectId))
            return false;

        // Check allocation if provided
        if (allocationPercentage.HasValue)
        {
            var currentAllocation = await GetUserTotalAllocationAsync(userId, DateTime.Now);
            if (currentAllocation + allocationPercentage.Value > 100)
                return false;
        }

        return true;
    }

    public async Task<bool> HasUserRoleInProjectAsync(Guid userId, Guid projectId, string role)
    {
        return await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .AnyAsync(ptm => ptm.UserId == userId &&
                            ptm.ProjectId == projectId &&
                            ptm.Role == role &&
                            ptm.IsActive);
    }

    private IQueryable<ProjectTeamMember> ApplyFilters(IQueryable<ProjectTeamMember> query, ProjectTeamMemberFilterDto filter)
    {
        if (filter.ProjectId.HasValue)
            query = query.Where(ptm => ptm.ProjectId == filter.ProjectId.Value);

        if (filter.UserId.HasValue)
            query = query.Where(ptm => ptm.UserId == filter.UserId.Value);

        if (!string.IsNullOrEmpty(filter.Role))
            query = query.Where(ptm => ptm.Role == filter.Role);

        if (filter.IsActive.HasValue)
            query = query.Where(ptm => ptm.IsActive == filter.IsActive.Value);

        if (filter.StartDateFrom.HasValue)
            query = query.Where(ptm => ptm.StartDate >= filter.StartDateFrom.Value);

        if (filter.StartDateTo.HasValue)
            query = query.Where(ptm => ptm.StartDate <= filter.StartDateTo.Value);

        if (filter.EndDateFrom.HasValue)
            query = query.Where(ptm => ptm.EndDate >= filter.EndDateFrom.Value);

        if (filter.EndDateTo.HasValue)
            query = query.Where(ptm => ptm.EndDate <= filter.EndDateTo.Value);

        if (filter.MinAllocation.HasValue)
            query = query.Where(ptm => ptm.AllocationPercentage >= filter.MinAllocation.Value);

        if (filter.MaxAllocation.HasValue)
            query = query.Where(ptm => ptm.AllocationPercentage <= filter.MaxAllocation.Value);

        return query;
    }

    private async Task<decimal> GetUserTotalAllocationAsync(Guid userId, DateTime date, Guid? excludeMemberId = null)
    {
        var query = _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .Where(ptm => ptm.UserId == userId &&
                         ptm.IsActive &&
                         ptm.StartDate <= date &&
                         (!ptm.EndDate.HasValue || ptm.EndDate.Value >= date));

        if (excludeMemberId.HasValue)
            query = query.Where(ptm => ptm.Id != excludeMemberId.Value);

        var allocations = await query.Select(ptm => ptm.AllocationPercentage ?? 0).ToListAsync();

        return allocations.Sum();
    }

    private ProjectTeamMemberDetailDto MapToDetailDto(ProjectTeamMember member)
    {
        var dto = _mapper.Map<ProjectTeamMemberDetailDto>(member);

        // Additional mappings not handled by AutoMapper
        dto.UserName = member.User.Name;
        dto.UserEmail = member.User.Email;
        dto.UserJobTitle = member.User.JobTitle;
        dto.AssignedAt = member.CreatedAt;
        dto.AssignedBy = member.CreatedBy;

        return dto;
    }
}