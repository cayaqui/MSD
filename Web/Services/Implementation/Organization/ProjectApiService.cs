using Core.DTOs.Common;
using Core.DTOs.Organization.Project;
using Core.DTOs.Auth.ProjectTeamMembers;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Organization;

namespace Web.Services.Implementation.Organization;

/// <summary>
/// Service implementation for project API operations
/// </summary>
public class ProjectApiService : IProjectApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private const string BaseEndpoint = "api/projects";

    public ProjectApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // Query Operations

    public async Task<PagedResult<ProjectListDto>> GetProjectsAsync(ProjectFilterDto? filter = null, int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool isAscending = true, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting projects - Page: {pageNumber}, Size: {pageSize}");
            
            var queryParams = new Dictionary<string, string>
            {
                ["pageNumber"] = pageNumber.ToString(),
                ["pageSize"] = pageSize.ToString(),
                ["isAscending"] = isAscending.ToString()
            };

            if (!string.IsNullOrEmpty(sortBy))
                queryParams["sortBy"] = sortBy;

            // Add filter parameters if provided
            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                    queryParams["searchTerm"] = filter.SearchTerm;
                
                if (!string.IsNullOrEmpty(filter.Status))
                    queryParams["status"] = filter.Status;
                
                if (filter.CompanyId.HasValue)
                    queryParams["companyId"] = filter.CompanyId.Value.ToString();
                
                if (filter.OperationId.HasValue)
                    queryParams["operationId"] = filter.OperationId.Value.ToString();
                
                if (!string.IsNullOrEmpty(filter.ProjectManagerId))
                    queryParams["projectManagerId"] = filter.ProjectManagerId;
                
                if (filter.StartDateFrom.HasValue)
                    queryParams["startDateFrom"] = filter.StartDateFrom.Value.ToString("O");
                
                if (filter.StartDateTo.HasValue)
                    queryParams["startDateTo"] = filter.StartDateTo.Value.ToString("O");
                
                if (filter.EndDateFrom.HasValue)
                    queryParams["endDateFrom"] = filter.EndDateFrom.Value.ToString("O");
                
                if (filter.EndDateTo.HasValue)
                    queryParams["endDateTo"] = filter.EndDateTo.Value.ToString("O");
            }

            return await _apiService.GetAsync<PagedResult<ProjectListDto>>($"{BaseEndpoint}?{BuildQueryString(queryParams)}") 
                ?? new PagedResult<ProjectListDto>(new List<ProjectListDto>(), pageNumber, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting projects");
            return new PagedResult<ProjectListDto>(new List<ProjectListDto>(), pageNumber, pageSize);
        }
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting project by ID: {projectId}");
            return await _apiService.GetAsync<ProjectDto>($"{BaseEndpoint}/{projectId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting project {projectId}");
            return null;
        }
    }

    public async Task<ProjectSummaryDto?> GetProjectsSummaryAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting projects summary");
            return await _apiService.GetAsync<ProjectSummaryDto>($"{BaseEndpoint}/summary");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting projects summary");
            return null;
        }
    }

    public async Task<List<ProjectTeamMemberDto>?> GetProjectTeamAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting project team for ID: {projectId}");
            return await _apiService.GetAsync<List<ProjectTeamMemberDto>>($"{BaseEndpoint}/{projectId}/team");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting project team for {projectId}");
            return null;
        }
    }

    public async Task<List<ProjectStatusHistoryDto>?> GetProjectStatusHistoryAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting project status history for ID: {projectId}");
            return await _apiService.GetAsync<List<ProjectStatusHistoryDto>>($"{BaseEndpoint}/{projectId}/status-history");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting project status history for {projectId}");
            return null;
        }
    }

    // Command Operations

    public async Task<ProjectDto?> CreateProjectAsync(CreateProjectDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Creating new project: {dto.Name}");
            return await _apiService.PostAsync<CreateProjectDto, ProjectDto>(BaseEndpoint, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating project: {dto.Name}");
            return null;
        }
    }

    public async Task<ProjectDto?> UpdateProjectAsync(Guid projectId, UpdateProjectDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating project: {projectId}");
            return await _apiService.PutAsync<UpdateProjectDto, ProjectDto>($"{BaseEndpoint}/{projectId}", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating project: {projectId}");
            return null;
        }
    }

    public async Task<ProjectDto?> ChangeProjectStatusAsync(Guid projectId, ChangeProjectStatusDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Changing project status: {projectId} to {dto.Status}");
            return await _apiService.PutAsync<ChangeProjectStatusDto, ProjectDto>($"{BaseEndpoint}/{projectId}/status", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error changing project status: {projectId}");
            return null;
        }
    }

    public async Task<ProjectDto?> HoldProjectAsync(Guid projectId, HoldProjectDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Putting project on hold: {projectId}");
            return await _apiService.PostAsync<HoldProjectDto, ProjectDto>($"{BaseEndpoint}/{projectId}/hold", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error putting project on hold: {projectId}");
            return null;
        }
    }

    public async Task<ProjectDto?> CancelProjectAsync(Guid projectId, CancelProjectDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Cancelling project: {projectId}");
            return await _apiService.PostAsync<CancelProjectDto, ProjectDto>($"{BaseEndpoint}/{projectId}/cancel", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error cancelling project: {projectId}");
            return null;
        }
    }

    public async Task<ProjectDto?> CompleteProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Completing project: {projectId}");
            return await _apiService.PostAsync<object, ProjectDto>($"{BaseEndpoint}/{projectId}/complete", new { });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error completing project: {projectId}");
            return null;
        }
    }

    public async Task<ProjectDto?> UpdateProjectProgressAsync(Guid projectId, UpdateProjectProgressDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating project progress: {projectId}");
            return await _apiService.PutAsync<UpdateProjectProgressDto, ProjectDto>($"{BaseEndpoint}/{projectId}/progress", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating project progress: {projectId}");
            return null;
        }
    }

    public async Task<bool> DeleteProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Deleting project: {projectId}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{projectId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting project: {projectId}");
            return false;
        }
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        return string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
    }
}