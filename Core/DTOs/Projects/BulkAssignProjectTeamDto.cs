namespace Core.DTOs.Projects
{
    public class BulkAssignProjectTeamDto
    {
        public Guid ProjectId { get; set; }
        public List<AssignUserDto> Users { get; set; } = new();
    }
}
