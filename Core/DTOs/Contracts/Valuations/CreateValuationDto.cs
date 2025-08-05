using Core.DTOs.Contracts.ValuationItems;

namespace Core.DTOs.Contracts.Valuations;

public class CreateValuationDto
{
    public string ValuationNumber { get; set; } = string.Empty;
    public int ValuationPeriod { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public Guid ContractId { get; set; }
    
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
    public DateTime ValuationDate { get; set; }
    public DateTime? DueDate { get; set; }
    
    public decimal MaterialsOnSite { get; set; }
    public decimal MaterialsOffSite { get; set; }
    
    public List<CreateValuationItemDto> Items { get; set; } = new();
    
    public string Notes { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
