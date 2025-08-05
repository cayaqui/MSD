namespace Core.Enums.Cost;
/// <summary>
/// EVM Status based on performance indices
/// </summary>
public enum EVMStatus
{
    OnTrack = 1,      // CPI & SPI >= 0.95
    Warning = 2,      // CPI or SPI between 0.90-0.95
    AtRisk = 3,       // CPI or SPI between 0.80-0.90
    Critical = 4      // CPI or SPI < 0.80
}
