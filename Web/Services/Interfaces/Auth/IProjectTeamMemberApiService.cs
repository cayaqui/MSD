using Core.DTOs.Common;
using Core.DTOs.Auth.ProjectTeamMembers;

namespace Web.Services.Interfaces.Auth;

/// <summary>
/// Interface for project team member API operations
/// </summary>
public interface IProjectTeamMemberApiService
{
    // Query Operations
    Task<PagedResult<ProjectTeamMemberDetailDto>> GetProjectTeamMembersAsync(ProjectTeamMemberFilterDto? filter = null, CancellationToken cancellationToken = default);
    Task<ProjectTeamMemberDetailDto?> GetProjectTeamMemberByIdAsync(Guid teamMemberId, CancellationToken cancellationToken = default);
    Task<List<ProjectTeamMemberDetailDto>?> GetProjectTeamAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<List<ProjectTeamMemberDetailDto>?> GetUserProjectsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserAvailabilityDto?> GetUserAvailabilityAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<List<TeamAllocationReportDto>?> GetAllocationReportAsync(DateTime? date = null, Guid? projectId = null, Guid? userId = null, CancellationToken cancellationToken = default);
    
    // Command Operations
    Task<ProjectTeamMemberDetailDto?> AddTeamMemberAsync(Guid projectId, AssignProjectTeamMemberDto dto, CancellationToken cancellationToken = default);
    Task<ProjectTeamMemberDetailDto?> UpdateTeamMemberAsync(Guid teamMemberId, UpdateProjectTeamMemberDto dto, CancellationToken cancellationToken = default);
    Task<bool> RemoveTeamMemberAsync(Guid teamMemberId, CancellationToken cancellationToken = default);
    
    // Bulk Operations
    Task<BulkAssignResultDto?> BulkAssignTeamMembersAsync(BulkAssignProjectTeamDto dto, CancellationToken cancellationToken = default);
    Task<int> RemoveAllFromProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    
    // Allocation Management
    Task<ProjectTeamMemberDetailDto?> UpdateAllocationAsync(Guid teamMemberId, decimal allocationPercentage, CancellationToken cancellationToken = default);
    
    // Transfer and Extension
    Task<ProjectTeamMemberDetailDto?> TransferTeamMemberAsync(Guid teamMemberId, TransferTeamMemberDto dto, CancellationToken cancellationToken = default);
    Task<ProjectTeamMemberDetailDto?> ExtendAssignmentAsync(Guid teamMemberId, DateTime newEndDate, CancellationToken cancellationToken = default);
    
    // Validation
    Task<bool> CheckUserAssignmentAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default);
    Task<AssignmentValidationResultDto?> CanAssignUserAsync(Guid userId, Guid projectId, decimal? allocationPercentage = null, CancellationToken cancellationToken = default);
    Task<bool> CheckUserRoleAsync(Guid userId, Guid projectId, string role, CancellationToken cancellationToken = default);
}