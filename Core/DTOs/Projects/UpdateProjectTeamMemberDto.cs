namespace Core.DTOs.Projects
{
    public class UpdateProjectTeamMemberDto
    {
        public string? Role { get; set; }
        public decimal? AllocationPercentage { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
