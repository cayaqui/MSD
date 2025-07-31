namespace Core.DTOs.Projects
{
    public class ProjectAllocationDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectCode { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public decimal AllocationPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
