namespace Core.Enums.Projects;
#region Planning Package Enums
/// <summary>
/// Planning Package Status
/// </summary>
public enum PlanningPackageStatus
{
    Future = 1,           // Far future planning
    NearTerm = 2,         // Within 60 days of conversion
    ReadyForConversion = 3, // Ready to convert to work packages
    Converting = 4,       // In process of conversion
    Converted = 5         // Already converted
}
#endregion