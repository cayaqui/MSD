using Core.DTOs.Organization.Project;

namespace Core.DTOs.Auth.ProjectTeamMembers
{
    public class TeamAllocationReportDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public decimal TotalAllocation { get; set; }
        public int ProjectCount { get; set; }
        public bool IsOverAllocated { get; set; }
        public List<ProjectAllocationDto> Projects { get; set; } = new();
    }
}
