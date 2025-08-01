using Application.Common.Exceptions;
using Application.Interfaces.Auth;
using Application.Interfaces.Setup;
using Core.Enums.Setup;
using Domain.Entities.Projects;
using InvalidOperationException = Application.Common.Exceptions.InvalidOperationException;

namespace Application.Services.Setup;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService,
        ILogger<ProjectService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<PagedResult<ProjectListDto>> GetPagedAsync(int pageNumber, int pageSize, ProjectFilterDto? filter = null)
    {
        var query = _unitOfWork.Repository<Project>()
            .Query()
            .Include(p => p.Operation)
            .Include(p => p.ProjectTeamMembers)
            .Where(p => !p.IsDeleted);

        // Apply filters
        if (filter != null)
        {
            query = ApplyFilters(query, filter);
        }

        // Apply user context filter if requested
        if (filter?.OnlyMyProjects == true && !string.IsNullOrEmpty(_currentUserService.UserId))
        {
            var user = await _unitOfWork.Repository<User>()
                .Query()
                .FirstOrDefaultAsync(u => u.EntraId == _currentUserService.UserId);

            if (user != null)
            {
                query = query.Where(p => p.ProjectTeamMembers.Any(ptm =>
                    ptm.UserId == user.Id && ptm.IsActive));
            }
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply ordering
        query = query.OrderByDescending(p => p.CreatedAt);

        // Apply pagination
        var projects = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Map to DTOs
        var projectDtos = new List<ProjectListDto>();
        foreach (var project in projects)
        {
            var dto = _mapper.Map<ProjectListDto>(project);

            // Add user role if authenticated
            if (!string.IsNullOrEmpty(_currentUserService.UserId))
            {
                var user = await _unitOfWork.Repository<User>()
                    .Query()
                    .FirstOrDefaultAsync(u => u.EntraId == _currentUserService.UserId);

                if (user != null)
                {
                    var membership = project.ProjectTeamMembers
                        .FirstOrDefault(ptm => ptm.UserId == user.Id && ptm.IsActive);

                    dto.UserRole = membership?.Role;
                }
            }

            projectDtos.Add(dto);
        }

        return new PagedResult<ProjectListDto>
        {
            Items = projectDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<IEnumerable<ProjectListDto>> GetUserProjectsAsync()
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
            return Enumerable.Empty<ProjectListDto>();

        var user = await _unitOfWork.Repository<User>()
            .Query()
            .FirstOrDefaultAsync(u => u.EntraId == _currentUserService.UserId);

        if (user == null)
            return Enumerable.Empty<ProjectListDto>();

        var projects = await _unitOfWork.Repository<Project>()
            .Query()
            .Include(p => p.Operation)
            .Include(p => p.ProjectTeamMembers)
            .Where(p => !p.IsDeleted &&
                       p.ProjectTeamMembers.Any(ptm => ptm.UserId == user.Id && ptm.IsActive))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var projectDtos = projects.Select(p =>
        {
            var dto = _mapper.Map<ProjectListDto>(p);
            dto.UserRole = p.ProjectTeamMembers
                .FirstOrDefault(ptm => ptm.UserId == user.Id && ptm.IsActive)?.Role;
            return dto;
        }).ToList();

        return projectDtos;
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        // Check user has access to this project
        if (!await _currentUserService.HasProjectAccessAsync(id))
            throw new ForbiddenAccessException();

        var project = await _unitOfWork.Repository<Project>()
            .Query()
            .Include(p => p.Operation)
            .Include(p => p.ProjectTeamMembers)
            //.Include(p => p.Phases)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        return project != null ? _mapper.Map<ProjectDto>(project) : null;
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
    {
        // Validate operation exists
        var operation = await _unitOfWork.Repository<Operation>()
            .Query()
            .FirstOrDefaultAsync(o => o.Id == dto.OperationId && !o.IsDeleted);

        if (operation == null)
            throw new NotFoundException(nameof(Operation), dto.OperationId);

        // Create project
        var project = new Project(
            dto.Code,
            dto.Name,
            dto.Description ?? string.Empty,
            dto.OperationId,
            dto.PlannedStartDate,
            dto.PlannedEndDate,
            dto.TotalBudget,
            dto.Currency,
            dto.Location ?? string.Empty
        );

        // Set additional properties
        if (!string.IsNullOrEmpty(dto.Client))
            project.UpdateDetails(dto.Name, dto.Description, dto.Location, dto.Client, dto.ContractNumber, dto.Currency);

        if (!string.IsNullOrEmpty(dto.WBSCode))
            project.UpdateWbsProfile(dto.WBSCode, dto.Client, dto.ContractNumber);

        await _unitOfWork.Repository<Project>().AddAsync(project);

        // Add current user as Project Manager if authenticated
        if (!string.IsNullOrEmpty(_currentUserService.UserId))
        {
            var user = await _unitOfWork.Repository<User>()
                .Query()
                .FirstOrDefaultAsync(u => u.EntraId == _currentUserService.UserId);

            if (user != null)
            {
                var teamMember = new ProjectTeamMember(
                    project.Id,
                    user.Id,
                    ProjectRoles.ProjectManager,
                    DateTime.UtcNow
                );

                await _unitOfWork.Repository<ProjectTeamMember>().AddAsync(teamMember);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Project {ProjectCode} created with ID {ProjectId}", project.Code, project.Id);

        return await GetByIdAsync(project.Id) ?? throw new InvalidOperationException("Failed to retrieve created project");
    }

    public async Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectDto dto)
    {
        // Check user has at least TeamLead access
        if (!await _currentUserService.HasProjectAccessAsync(id, ProjectRoles.TeamLead))
            throw new ForbiddenAccessException();

        var project = await _unitOfWork.Repository<Project>()
            .Query()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (project == null)
            throw new NotFoundException(nameof(Project), id);

        // Update basic details if provided
        if (!string.IsNullOrEmpty(dto.Name) || dto.Location != null || dto.Client != null)
        {
            project.UpdateDetails(
                dto.Name ?? project.Name,
                dto.Description ?? project.Description,
                dto.Location ?? project.Location,
                dto.Client ?? project.Client,
                dto.ContractNumber ?? project.ContractNumber,
                project.Currency
            );
        }

        // Update dates if provided
        if (dto.PlannedStartDate.HasValue || dto.PlannedEndDate.HasValue)
        {
            project.UpdatePlannedDates(
                dto.PlannedStartDate ?? project.PlannedStartDate,
                dto.PlannedEndDate ?? project.PlannedEndDate
            );
        }

        // Update budget if provided
        if (dto.TotalBudget.HasValue)
        {
            project.UpdateBudget(dto.TotalBudget.Value);
        }

        // Update status if provided (requires ProjectManager role)
        if (!string.IsNullOrEmpty(dto.Status))
        {
            if (!await _currentUserService.HasProjectAccessAsync(id, ProjectRoles.ProjectManager))
                throw new ForbiddenAccessException("Only Project Managers can change project status");

            project.UpdateStatus(Enum.Parse<ProjectStatus>(dto.Status));
        }

        // Update active status if provided
        if (dto.IsActive.HasValue)
        {
            if (dto.IsActive.Value)
                project.Activate();
            else
                project.Deactivate();
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Project {ProjectId} updated", id);

        return await GetByIdAsync(id) ?? throw new InvalidOperationException("Failed to retrieve updated project");
    }

    public async Task DeleteAsync(Guid id)
    {
        // Check user has ProjectManager access
        if (!await _currentUserService.HasProjectAccessAsync(id, ProjectRoles.ProjectManager))
            throw new ForbiddenAccessException();

        var project = await _unitOfWork.Repository<Project>()
            .Query()
            //.Include(p => p.Phases)
            //.Include(p => p.WorkPackages)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (project == null)
            throw new NotFoundException(nameof(Project), id);

        // Check if project can be deleted
        //if (project.Phases.Any() || project.WorkPackages.Any())
        //    throw new BadRequestException("Cannot delete project with existing phases or work packages");

        project.IsDeleted = true;
        project.DeletedAt = DateTime.UtcNow;
        project.DeletedBy = _currentUserService.UserId;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Project {ProjectId} deleted", id);
    }

    public async Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null)
    {
        var query = _unitOfWork.Repository<Project>()
            .Query()
            .Where(p => p.Code == code && !p.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return !await query.AnyAsync();
    }

    public async Task AddTeamMemberAsync(Guid projectId, Guid userId, string role)
    {
        // Check user has permission to manage team
        if (!await _currentUserService.HasProjectAccessAsync(projectId, ProjectRoles.ProjectManager))
            throw new ForbiddenAccessException();

        // Validate project exists
        var project = await _unitOfWork.Repository<Project>()
            .Query()
            .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted);

        if (project == null)
            throw new NotFoundException(nameof(Project), projectId);

        // Validate user exists
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

        if (user == null)
            throw new NotFoundException(nameof(User), userId);

        // Check if user is already a member
        var existingMember = await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .FirstOrDefaultAsync(ptm => ptm.ProjectId == projectId && ptm.UserId == userId);

        if (existingMember != null)
        {
            if (existingMember.IsActive)
                throw new BadRequestException("User is already a member of this project");

            // Reactivate if previously removed
            existingMember.IsActive = true;
            existingMember.Role = role;
            existingMember.StartDate = DateTime.UtcNow;
            existingMember.EndDate = null;
        }
        else
        {
            // Add new member
            var teamMember = new ProjectTeamMember(projectId, userId, role, DateTime.UtcNow);
            await _unitOfWork.Repository<ProjectTeamMember>().AddAsync(teamMember);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User {UserId} added to project {ProjectId} as {Role}", userId, projectId, role);
    }

    public async Task RemoveTeamMemberAsync(Guid projectId, Guid userId)
    {
        // Check user has permission to manage team
        if (!await _currentUserService.HasProjectAccessAsync(projectId, ProjectRoles.ProjectManager))
            throw new ForbiddenAccessException();

        var teamMember = await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .FirstOrDefaultAsync(ptm => ptm.ProjectId == projectId && ptm.UserId == userId && ptm.IsActive);

        if (teamMember == null)
            throw new NotFoundException("Team member not found in project");

        // Don't allow removing the last Project Manager
        if (teamMember.Role == ProjectRoles.ProjectManager)
        {
            var otherPMs = await _unitOfWork.Repository<ProjectTeamMember>()
                .Query()
                .CountAsync(ptm =>
                    ptm.ProjectId == projectId &&
                    ptm.UserId != userId &&
                    ptm.Role == ProjectRoles.ProjectManager &&
                    ptm.IsActive);

            if (otherPMs == 0)
                throw new BadRequestException("Cannot remove the last Project Manager");
        }

        teamMember.IsActive = false;
        teamMember.EndDate = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User {UserId} removed from project {ProjectId}", userId, projectId);
    }

    public async Task UpdateProgressAsync(Guid projectId, decimal percentage)
    {
        // Check user has at least TeamMember access
        if (!await _currentUserService.HasProjectAccessAsync(projectId, ProjectRoles.TeamMember))
            throw new ForbiddenAccessException();

        var project = await _unitOfWork.Repository<Project>()
            .Query()
            .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted);

        if (project == null)
            throw new NotFoundException(nameof(Project), projectId);

        project.UpdateProgress(percentage);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Project {ProjectId} progress updated to {Percentage}%", projectId, percentage);
    }

    private IQueryable<Project> ApplyFilters(IQueryable<Project> query, ProjectFilterDto filter)
    {
        // Search term
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Code.ToLower().Contains(searchTerm) ||
                p.Name.ToLower().Contains(searchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
        }

        // Operation filter
        if (filter.OperationId.HasValue)
        {
            query = query.Where(p => p.OperationId == filter.OperationId.Value);
        }

        // Company filter
        if (filter.CompanyId.HasValue)
        {
            query = query.Where(p => p.Operation.CompanyId == filter.CompanyId.Value);
        }

        // Status filter
        if (!string.IsNullOrEmpty(filter.Status))
        {
            var status = Enum.Parse<ProjectStatus>(filter.Status);
            query = query.Where(p => p.Status == status);
        }

        // Active filter
        if (filter.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == filter.IsActive.Value);
        }

        // Date range filters
        if (filter.StartDateFrom.HasValue)
        {
            query = query.Where(p => p.PlannedStartDate >= filter.StartDateFrom.Value);
        }

        if (filter.StartDateTo.HasValue)
        {
            query = query.Where(p => p.PlannedStartDate <= filter.StartDateTo.Value);
        }

        if (filter.EndDateFrom.HasValue)
        {
            query = query.Where(p => p.PlannedEndDate >= filter.EndDateFrom.Value);
        }

        if (filter.EndDateTo.HasValue)
        {
            query = query.Where(p => p.PlannedEndDate <= filter.EndDateTo.Value);
        }

        // Budget filters
        if (filter.MinBudget.HasValue)
        {
            query = query.Where(p => p.TotalBudget >= filter.MinBudget.Value);
        }

        if (filter.MaxBudget.HasValue)
        {
            query = query.Where(p => p.TotalBudget <= filter.MaxBudget.Value);
        }

        if (!string.IsNullOrEmpty(filter.Currency))
        {
            query = query.Where(p => p.Currency == filter.Currency);
        }

        // Additional filters
        if (!string.IsNullOrEmpty(filter.Client))
        {
            query = query.Where(p => p.Client != null && p.Client.ToLower().Contains(filter.Client.ToLower()));
        }

        if (!string.IsNullOrEmpty(filter.Location))
        {
            query = query.Where(p => p.Location != null && p.Location.ToLower().Contains(filter.Location.ToLower()));
        }

        return query;
    }
}