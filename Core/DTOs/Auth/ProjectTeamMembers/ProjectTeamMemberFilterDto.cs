namespace Core.DTOs.Auth.ProjectTeamMembers
{
    public class ProjectTeamMemberFilterDto
    {
        public Guid? ProjectId { get; set; }
        public Guid? UserId { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public decimal? MinAllocation { get; set; }
        public decimal? MaxAllocation { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
