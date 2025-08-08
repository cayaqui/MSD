using Core.DTOs.Common;
using Core.DTOs.Auth.ProjectTeamMembers;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Auth;

namespace Web.Services.Implementation.Auth;

/// <summary>
/// Service implementation for project team member API operations
/// </summary>
public class ProjectTeamMemberApiService : IProjectTeamMemberApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private const string BaseEndpoint = "api/project-team-members";

    public ProjectTeamMemberApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // Query Operations
    public async Task<PagedResult<ProjectTeamMemberDetailDto>> GetProjectTeamMembersAsync(ProjectTeamMemberFilterDto? filter = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Searching project team members with filter");
            filter ??= new ProjectTeamMemberFilterDto();
            return await _apiService.PostAsync<ProjectTeamMemberFilterDto, PagedResult<ProjectTeamMemberDetailDto>>($"{BaseEndpoint}/search", filter) 
                ?? new PagedResult<ProjectTeamMemberDetailDto>(new List<ProjectTeamMemberDetailDto>(), filter.PageNumber, filter.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching project team members");
            return new PagedResult<ProjectTeamMemberDetailDto>(new List<ProjectTeamMemberDetailDto>(), filter?.PageNumber ?? 1, filter?.PageSize ?? 10);
        }
    }

    public async Task<ProjectTeamMemberDetailDto?> GetProjectTeamMemberByIdAsync(Guid teamMemberId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting project team member by ID: {teamMemberId}");
            return await _apiService.GetAsync<ProjectTeamMemberDetailDto>($"{BaseEndpoint}/{teamMemberId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting project team member {teamMemberId}");
            return null;
        }
    }

    public async Task<List<ProjectTeamMemberDetailDto>?> GetProjectTeamAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting team members for project: {projectId}");
            return await _apiService.GetAsync<List<ProjectTeamMemberDetailDto>>($"{BaseEndpoint}/project/{projectId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting team members for project {projectId}");
            return null;
        }
    }

    public async Task<List<ProjectTeamMemberDetailDto>?> GetUserProjectsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting project assignments for user: {userId}");
            return await _apiService.GetAsync<List<ProjectTeamMemberDetailDto>>($"{BaseEndpoint}/user/{userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting project assignments for user {userId}");
            return null;
        }
    }

    public async Task<UserAvailabilityDto?> GetUserAvailabilityAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting availability for user: {userId}");
            var query = "";
            if (startDate.HasValue || endDate.HasValue)
            {
                var queryParams = new List<string>();
                if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
                query = "?" + string.Join("&", queryParams);
            }
            return await _apiService.GetAsync<UserAvailabilityDto>($"{BaseEndpoint}/user/{userId}/availability{query}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting availability for user {userId}");
            return null;
        }
    }

    public async Task<List<TeamAllocationReportDto>?> GetAllocationReportAsync(DateTime? date = null, Guid? projectId = null, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting allocation report");
            var queryParams = new List<string>();
            if (date.HasValue) queryParams.Add($"date={date.Value:yyyy-MM-dd}");
            if (projectId.HasValue) queryParams.Add($"projectId={projectId.Value}");
            if (userId.HasValue) queryParams.Add($"userId={userId.Value}");
            var query = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            
            return await _apiService.GetAsync<List<TeamAllocationReportDto>>($"{BaseEndpoint}/allocation-report{query}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting allocation report");
            return null;
        }
    }

    // Command Operations
    public async Task<ProjectTeamMemberDetailDto?> AddTeamMemberAsync(Guid projectId, AssignProjectTeamMemberDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Adding team member to project: {projectId}");
            return await _apiService.PostAsync<AssignProjectTeamMemberDto, ProjectTeamMemberDetailDto>($"{BaseEndpoint}/project/{projectId}/assign", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding team member to project {projectId}");
            return null;
        }
    }

    public async Task<ProjectTeamMemberDetailDto?> UpdateTeamMemberAsync(Guid teamMemberId, UpdateProjectTeamMemberDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating team member: {teamMemberId}");
            return await _apiService.PutAsync<UpdateProjectTeamMemberDto, ProjectTeamMemberDetailDto>($"{BaseEndpoint}/{teamMemberId}", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating team member {teamMemberId}");
            return null;
        }
    }

    public async Task<bool> RemoveTeamMemberAsync(Guid teamMemberId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Removing team member: {teamMemberId}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{teamMemberId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error removing team member {teamMemberId}");
            return false;
        }
    }

    // Bulk Operations
    public async Task<BulkAssignResultDto?> BulkAssignTeamMembersAsync(BulkAssignProjectTeamDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Bulk assigning {dto.Assignments.Count} team members");
            return await _apiService.PostAsync<BulkAssignProjectTeamDto, BulkAssignResultDto>($"{BaseEndpoint}/bulk-assign", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk assigning team members");
            return null;
        }
    }

    public async Task<int> RemoveAllFromProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Removing all team members from project: {projectId}");
            var result = await _apiService.DeleteAsync($"{BaseEndpoint}/project/{projectId}/all");
            return result ? 1 : 0; // API returns count, but we don't have access to it directly
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error removing all team members from project {projectId}");
            return 0;
        }
    }

    // Allocation Management
    public async Task<ProjectTeamMemberDetailDto?> UpdateAllocationAsync(Guid teamMemberId, decimal allocationPercentage, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating allocation for team member: {teamMemberId} to {allocationPercentage}%");
            return await _apiService.PutAsync<object, ProjectTeamMemberDetailDto>($"{BaseEndpoint}/{teamMemberId}/allocation", new { AllocationPercentage = allocationPercentage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating allocation for team member {teamMemberId}");
            return null;
        }
    }

    // Transfer and Extension
    public async Task<ProjectTeamMemberDetailDto?> TransferTeamMemberAsync(Guid teamMemberId, TransferTeamMemberDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Transferring team member: {teamMemberId} to project {dto.NewProjectId}");
            return await _apiService.PostAsync<TransferTeamMemberDto, ProjectTeamMemberDetailDto>($"{BaseEndpoint}/{teamMemberId}/transfer", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error transferring team member {teamMemberId}");
            return null;
        }
    }

    public async Task<ProjectTeamMemberDetailDto?> ExtendAssignmentAsync(Guid teamMemberId, DateTime newEndDate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Extending assignment for team member: {teamMemberId} to {newEndDate:yyyy-MM-dd}");
            return await _apiService.PostAsync<object, ProjectTeamMemberDetailDto>($"{BaseEndpoint}/{teamMemberId}/extend", new { NewEndDate = newEndDate });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error extending assignment for team member {teamMemberId}");
            return null;
        }
    }

    // Validation
    public async Task<bool> CheckUserAssignmentAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Checking if user {userId} is assigned to project {projectId}");
            return await _apiService.GetAsync<bool>($"{BaseEndpoint}/check-assignment/{userId}/{projectId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking user assignment");
            return false;
        }
    }

    public async Task<AssignmentValidationResultDto?> CanAssignUserAsync(Guid userId, Guid projectId, decimal? allocationPercentage = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Checking if user {userId} can be assigned to project {projectId}");
            return await _apiService.PostAsync<object, AssignmentValidationResultDto>($"{BaseEndpoint}/can-assign", new { UserId = userId, ProjectId = projectId, AllocationPercentage = allocationPercentage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking if user can be assigned");
            return null;
        }
    }

    public async Task<bool> CheckUserRoleAsync(Guid userId, Guid projectId, string role, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Checking if user {userId} has role {role} in project {projectId}");
            return await _apiService.GetAsync<bool>($"{BaseEndpoint}/check-role/{userId}/{projectId}/{Uri.EscapeDataString(role)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking user role");
            return false;
        }
    }
}