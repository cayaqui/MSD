namespace Core.DTOs.Projects
{
    public class UserAvailabilityDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal TotalAllocation { get; set; }
        public decimal AvailableCapacity { get; set; }
        public List<ProjectAllocationDto> ProjectAllocations { get; set; } = new();
    }
}
