namespace Core.Enums.Cost;

// Enums
public enum CostType
{
    Labor,
    Material,
    Equipment,
    Subcontract,
    Indirect,
    Other
}
// Implemeny extension methods if needed
public static class CostTypeExtensions
{
    public static string ToFriendlyString(this CostType costType)
    {
        return costType switch
        {
            CostType.Labor => "Labor",
            CostType.Material => "Material",
            CostType.Equipment => "Equipment",
            CostType.Subcontract => "Subcontract",
            CostType.Indirect => "Indirect Costs",
            CostType.Other => "Other Costs",
            _ => "Unknown"
        };
    }
    public static CostType GetFromString(string value)
    {
        return value switch
        {
            "Labor" => CostType.Labor,
            "Material" => CostType.Material,
            "Equipment" => CostType.Equipment,
            "Subcontract" => CostType.Subcontract,
            "Indirect Costs" => CostType.Indirect,
            _ => CostType.Other,
        };
    }
}