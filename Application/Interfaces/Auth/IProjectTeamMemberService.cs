using Core.DTOs.Auth.ProjectTeamMembers;
using Core.DTOs.Common;
using Core.DTOs.Organization.Project;

namespace Application.Interfaces.Auth;

public interface IProjectTeamMemberService
{
    // Read operations
    Task<PagedResult<ProjectTeamMemberDetailDto>> GetPagedAsync(ProjectTeamMemberFilterDto filter);
    Task<ProjectTeamMemberDetailDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProjectTeamMemberDetailDto>> GetByProjectAsync(Guid projectId);
    Task<IEnumerable<ProjectTeamMemberDetailDto>> GetByUserAsync(Guid userId);

    // Write operations
    Task<ProjectTeamMemberDetailDto> CreateAsync(Guid projectId, AssignProjectTeamMemberDto dto);
    Task<ProjectTeamMemberDetailDto?> UpdateAsync(Guid id, UpdateProjectTeamMemberDto dto);
    Task RemoveAsync(Guid id);

    // Bulk operations
    Task<int> BulkAssignAsync(BulkAssignProjectTeamDto dto);
    Task<int> RemoveAllFromProjectAsync(Guid projectId);

    // Allocation management
    Task<UserAvailabilityDto?> GetUserAvailabilityAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<TeamAllocationReportDto>> GetAllocationReportAsync(DateTime? date = null, Guid? projectId = null, Guid? userId = null);
    Task<ProjectTeamMemberDetailDto?> UpdateAllocationAsync(Guid id, decimal allocationPercentage);

    // Assignment management
    Task<ProjectTeamMemberDetailDto?> TransferAsync(Guid id, TransferTeamMemberDto dto);
    Task<ProjectTeamMemberDetailDto?> ExtendAssignmentAsync(Guid id, DateTime newEndDate);

    // Validation
    Task<bool> IsUserAssignedToProjectAsync(Guid userId, Guid projectId);
    Task<bool> CanUserBeAssignedAsync(Guid userId, Guid projectId, decimal? allocationPercentage = null);
    Task<bool> HasUserRoleInProjectAsync(Guid userId, Guid projectId, string role);
}