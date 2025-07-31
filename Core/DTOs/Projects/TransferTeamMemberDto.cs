namespace Core.DTOs.Projects
{
    public class TransferTeamMemberDto
    {
        public Guid NewProjectId { get; set; }
        public string? NewRole { get; set; }
        public DateTime TransferDate { get; set; }
        public string? TransferReason { get; set; }
    }
}
