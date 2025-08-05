namespace Core.Enums.Cost;

public enum VariationType
{
    Addition = 1,       // Scope addition
    Omission = 2,       // Scope reduction
    Substitution = 3,   // Material/method change
    Acceleration = 4,   // Schedule compression
    Delay = 5,          // Time extension
    RateAdjustment = 6, // Unit rate change
    QuantityChange = 7, // Quantity variation
    Provisional = 8     // Provisional sum adjustment
}
