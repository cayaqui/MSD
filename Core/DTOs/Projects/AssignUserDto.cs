namespace Core.DTOs.Projects
{
    public class AssignUserDto
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public decimal? AllocationPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
