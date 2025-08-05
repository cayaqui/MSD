using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.Claims;

public class ClaimFilterDto
{
    public string? SearchTerm { get; set; }
    public Guid? ContractId { get; set; }
    public ClaimType? Type { get; set; }
    public ClaimStatus? Status { get; set; }
    public ClaimPriority? Priority { get; set; }
    public ClaimDirection? Direction { get; set; }
    public ClaimResolution? Resolution { get; set; }
    public DateTime? EventDateFrom { get; set; }
    public DateTime? EventDateTo { get; set; }
    public DateTime? SubmissionDateFrom { get; set; }
    public DateTime? SubmissionDateTo { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public bool? IsTimeBarred { get; set; }
    public bool? HasMerit { get; set; }
    public bool? IsActive { get; set; }
}
