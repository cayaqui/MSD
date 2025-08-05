using Core.DTOs.Contracts.ValuationItems;
using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.Valuations;

public class UpdateValuationDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ValuationStatus Status { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public decimal MaterialsOnSite { get; set; }
    public decimal MaterialsOffSite { get; set; }
    
    public List<UpdateValuationItemDto> Items { get; set; } = new();
    
    public string Notes { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
