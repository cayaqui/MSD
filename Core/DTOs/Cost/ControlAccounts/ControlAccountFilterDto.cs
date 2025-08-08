using Core.DTOs.Common;

namespace Core.DTOs.Cost.ControlAccounts;

public class ControlAccountFilterDto : BaseFilterDto
{
    public Guid? PhaseId { get; set; }
    public string? Status { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public bool? IsBaselined { get; set; }
    public bool? OnlyActive { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
    public override string? SortBy { get; set; } = "Code";
}