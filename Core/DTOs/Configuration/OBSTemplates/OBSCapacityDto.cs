namespace Core.DTOs.Configuration.OBSTemplates;

public class OBSCapacityDto
{
    public decimal TotalFTE { get; set; }
    public decimal AllocatedFTE { get; set; }
    public decimal AvailableFTE { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public Dictionary<string, decimal> CapacityByRole { get; set; } = new();
    public Dictionary<string, decimal> CapacityByProject { get; set; } = new();
}
