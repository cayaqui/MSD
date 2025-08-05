namespace Core.DTOs.Contracts.ContractMilestones;

public class MilestoneDependencyValidation
{
    public Guid MilestoneId { get; set; }
    public bool IsValid { get; set; }
    public List<string> Issues { get; set; } = new List<string>();
    public bool HasCircularDependency { get; set; }
    public List<Guid> CircularPath { get; set; } = new List<Guid>();
}